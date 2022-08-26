from datetime import datetime
import urllib3
import requests
import json
import sys

start = datetime.now()

doStore = sys.argv[1] == 'store' if len(sys.argv) > 1 else False
apiUrl = 'https://localhost:44384' if doStore else 'https://localhost:6001'
proxyPath = 'storeProxy' if doStore else 'apiProxy'
clientsPath = './src/services/storeClients.ts' if doStore else './src/services/clients.ts'

print('syncing api proxy...')
urllib3.disable_warnings() # Ignore insecure request warning
r = requests.get(apiUrl + '/swagger/v1/swagger.json', verify=False)
swagger = json.loads(r.text)
fileText: str = f"import http from './{'storeHttp' if doStore else 'http'}'\nimport objToFormData from './formData'\n\n"

def camelCase(word: str):
	return word[0].lower() + word[1:]

# Return TS type of function parameter or object property
def getType(schema: dict):
	type = '' if 'type' not in schema else schema['type']
	if type == '':
		if '$ref' not in schema: return 'any'
		return schema['$ref'].split('/')[-1]
	elif type == 'string':
		if 'format' in schema:
			format = schema['format']
			if format == 'date-time': return 'Date'
			elif format == 'binary': return 'IProxyFileParameter'
		return type
	elif type == 'integer' or type == 'number':
		if 'enum' in schema:
			return 'number'
		return 'number'
	elif type == 'array': return getType(schema['items']) + '[]'
	elif type == 'boolean': return 'boolean'
	elif type == 'object':
		if 'properties' in schema:
			propsText: str = '{ '
			required: list = [] if 'required' not in schema else schema['required']
			p: str
			for p in schema['properties']:
				prop = schema['properties'][p]
				propsText += f"{camelCase(p)}{'' if p in required else '?'}: {getType(prop)}, "
			if len(propsText) > 2: propsText = propsText[:-2]
			propsText += ' }'
			return propsText
		elif 'additionalProperties' in schema:
			if 'type' in schema['additionalProperties']:
				return schema['additionalProperties']['type']
			else: return 'any'

# Add TS return type to proxy client method
def addReturnType(method: dict) -> str:
	returnType = 'void'
	responses: dict = method['responses']['200']
	if 'content' in responses:
		schema: dict = responses['content']['application/json']['schema']
		if 'type' in schema:
			returnType = getType(schema)
		elif '$ref' in schema:
			returnType = schema['$ref'].split('/')[-1]

	global fileText
	fileText += f'): Promise<{returnType}> {{\n\t\t'
	return returnType

# Add TS parameters to proxy client method
def addParams(method: dict, mode: str, form: bool = False):
	global fileText
	params = []
	p: dict

	if 'parameters' in method[mode]:
		params = method[mode]['parameters']
		required: list = list(filter(lambda x: 'required' in x, params))
		optional: list = list(filter(lambda x: not 'required' in x, params))

		j = 0
		for p in required:
			if (mode == 'post' or mode == 'put') and j == 0: fileText += ', '
			fileText += f"{camelCase(p['name'])}: {getType(p['schema'])}, "
			j += 1
		if j > 0: fileText = fileText[:-2]

		if len(required) > 0 and len(optional) > 0: fileText += ', '
		j = 0
		for p in optional:
			fileText += f"{camelCase(p['name'])}?: {getType(p['schema'])}, "
			j += 1
		if j > 0: fileText = fileText[:-2]

	elif form:
		schema: dict = method[mode]['requestBody']['content']['multipart/form-data']['schema']
		props: dict = schema['properties']
		required: list = [] if 'required' not in schema else schema['required']
		propName: str
		prop: dict
		fileText += f"data{'' if len(required) > 0 else '?'}: {{ "

		j = 0
		for propName in required:
			prop = props[propName]
			fileText += f"{camelCase(propName.replace('.', '_'))}: {getType(prop)}, "
			j += 1
		if j > 0: fileText = fileText[:-2]

		j = 0
		for propName in props:
			if not propName in required:
				if len(required) > 0 and j == 0: fileText += ', '
				prop = props[propName]
				fileText += f"{camelCase(propName.replace('.', '_'))}?: {getType(prop)}, "
				j += 1
		if j > 0: fileText = fileText[:-2]

		fileText += ' }'

	returnType = addReturnType(method[mode])
	fileText += f"let url_ = '{path}?'\n\t\t"
	j = 0
	for p in params:
		fileText += f"if ({camelCase(p['name'])} != null) url_ += `{camelCase(p['name'])}=${{{camelCase(p['name'])}}}&`\n\t\t"
		j += 1
	fileText += "url_ = url_.replace(/[?&]$/, '')\n\n\t\t"
	return returnType

# Return default value for TS type
def getDefaultValue(dataType: str, allModels: dict) -> str:
	if dataType == 'string': return "''"
	elif dataType == 'number': return '0'
	elif dataType == 'boolean': return 'false'
	elif dataType == 'Date': return 'new Date()'
	elif '[]' in dataType: return '[]'
	elif dataType[0] == dataType[0].upper():
		if dataType == 'IProxyFileParameter': return "{ data: undefined, fileName: '' }"
		model: dict = allModels[dataType]
		if 'enum' in model: return '0'
		else: return f'{dataType}.newEmpty()'
	else: return 'undefined'

# Clients/Methods (API controllers/endpoints)
paths: dict = swagger['paths']
i = 0
clients = []
path: str
for path in paths:
	parts = path.split('/')
	routeName = parts[-1]
	clientName: str
	if len(parts) == 2: clientName = routeName
	else: clientName = parts[-2]
	clientName += 'Client'
	methodName = camelCase(routeName)

	if not '{' in clientName:
		# Create Client Class
		if not clientName in clients:
			clients.append(clientName)
			if not i == 0:
				fileText += '\n}\n\n'
			fileText += f'export class {clientName} {{\n\t'
		else: fileText += '\n\n\t'
		
		# Add Method
		fileText += f'async {methodName}('
		method: dict = paths[path]
		returnType: str = ''

		if 'get' in method or 'delete' in method:
			returnType = addParams(method, 'get' if 'get' in method else 'delete')
			fileText += f"""{'const response = ' if returnType != 'void' else ''}await http.request({{
			method: '{'GET' if 'get' in method else 'DELETE'}',
			url: url_,
			headers: {{
				'Accept': 'text/plain',
				'X-CSRF': '1'
			}}"""

		elif 'post' in method or 'put' in method:
			mode = 'post' if 'post' in method else 'put'
			schema: dict = {}
			form: bool = False

			if 'requestBody' in method[mode]:
				content: dict = method[mode]['requestBody']['content']
				if 'application/json' in content:
					schema = content['application/json']['schema']
					if 'type' in schema:
						fileText += f"data: {schema['items']['$ref'].split('/')[-1]}"
					else: fileText += f"data: {schema['$ref'].split('/')[-1]}"
				elif 'multipart/form-data' in content:
					form = True
					schema = content['multipart/form-data']['schema']
			returnType = addParams(method, mode, form)

			if form: fileText += 'const formData = objToFormData(data)\n\t\t'

			data: str = f"\n\t\t\tdata: {'formData' if form else 'JSON.stringify(data)'},"
			fileText += f"""{'const response = ' if returnType != 'void' else ''}await http.request({{
			method: '{mode.upper()}',
			url: url_,{data if 'requestBody' in method[mode] else ''}
			headers: {{
				'Content-Type': '{'multipart/form-data' if form else 'application/json'}',
				'Accept': 'text/plain',
				'X-CSRF': '1'
			}}"""

		fileText += f'\n\t\t}})'

		if returnType != 'void':
			fileText += '\n\n\t\t'
			if returnType[0].upper() == returnType[0]:
				if '[]' in returnType:
					fileText += f"""const list: {returnType} = []
		const responseList = response.data
		if (Array.isArray(responseList)) {{
			for (let item of responseList) list.push(new {returnType[:-2]}(item))
		}}
		return list"""
				else: fileText += f'return new {returnType}(response.data)'
			else: fileText += 'return response.data'

		fileText += '\n\t}'
		i += 1

fileText += '\n}\n\n'

# Models (C# classes and enums)
models: dict = swagger['components']['schemas']
modelName: str
for modelName in models:
	model: dict = models[modelName]
	# Enum
	if 'enum' in model:
		fileText += f'export enum {modelName} {{'
		enumLength = len(model['enum'])
		for i in range(enumLength):
			assignment = f"{model['x-enumNames'][i]} = {model['enum'][i]}" if 'x-enumNames' in model else f"{model['enum'][i]} = {i}"
			fileText += f"\n\t{assignment}{',' if i < enumLength - 1 else ''}"
		fileText += '\n}\n\n'

	# Class
	elif 'properties' in model:
		propsText: str = ''
		required: list = [] if 'required' not in model else model['required']
		p: str
		for p in model['properties']:
			prop = model['properties'][p]
			propsText += f"\n\t{p}{'' if p in required else '?'}: {getType(prop)}"
		
		# Interface
		fileText += f'export interface I{modelName} {{'
		fileText += '\n\t[x: string]: any'
		fileText += propsText
		fileText += '\n}\n'
		
		# Class
		fileText += f'export class {modelName} {{'

		# Class Properties
		fileText += '\n\t[x: string]: any'
		fileText += propsText

		# Class Constructor
		initializers: str = ''
		for p in model['properties']:
			tab = '' if len(required) > 0 else '\t'
			initializers += f"\n\t\t{tab}this.{p} = data.{p}"
		fileText += f"\n\n\tconstructor(data{'' if len(required) > 0 else '?'}: I{modelName}) {{"
		if len(required) > 0: fileText += initializers
		else: fileText += f'\n\t\tif (data) {{{initializers}\n\t\t}}'
		fileText += '\n\t}'

		# Class 'initEmpty' Method
		optParam = 'data?: { [x: string]: any, '
		for p in model['properties']:
			prop = model['properties'][p]
			optParam += f'{p}?: {getType(prop)}, '
		optParam = optParam[:-2] + ' }'
		fileText += f'\n\n\tstatic newEmpty({optParam}) {{\n\t\tlet empty: I{modelName} = {{'
		if len(required) == 0: fileText += '}'
		else:
			for p in model['properties']:
				if p in required:
					prop = model['properties'][p]
					defaultValue: str = 'undefined'
					if p in required: defaultValue = getDefaultValue(getType(prop), models)
					fileText += f'\n\t\t\t{p}: {defaultValue},'
			fileText = fileText[:-1]
			fileText += '\n\t\t}\n'
		fileText += '\n\t\tif (data != null)\n\t\t\tObject.keys(data).forEach(key => { if (data[key] != null) empty[key] = data[key] })'
		fileText += f'\n\n\t\treturn new {modelName}(empty)\n\t}}'

		fileText += '\n}\n\n'

# Interface required for all file upload methods
fileText += """export interface IProxyFileParameter {
	data: any
	fileName: string
}\n"""

# Write proxy file
with open(F'./src/services/{proxyPath}.ts', 'w') as file:
	file.write(fileText)
	file.close()

# Create client instances and write to file
fileText = ''
importsText: str = ''
instancesText: str = ''
client: str
for client in clients:
	importsText += f"import {{ {client} }} from './{proxyPath}'\n"
	instancesText += f'export const {camelCase(client)} = new {client}()\n'
with open(clientsPath, 'w') as file:
	file.write(f'{importsText}\n{instancesText}')
	file.close()

print(f'finished in {round((datetime.now() - start).total_seconds() * 1000.0)}ms')
