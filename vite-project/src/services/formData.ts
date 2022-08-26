function objToFormData(obj: Record<string, any>, parentKey?: string, data?: FormData) {
	let formData = data ?? new FormData()
	const objKeys = Object.keys(obj)

	if (objKeys.length === 2 && objKeys.includes('data') && objKeys.includes('fileName'))
		formData.append(parentKey!, obj.data, obj.fileName)
	else {
		objKeys.forEach(key => {
			const pascalKey = ((parentKey ? parentKey + '.' : '') + key[0].toUpperCase() + key.slice(1)).replaceAll('_', '.')
			const value = obj[key]

			switch (typeof value) {
				case 'bigint':
				case 'boolean':
				case 'number':
				case 'string':
					formData.append(pascalKey, value.toString())
					break
				case 'function':
					break
				case 'object':
					if (value != null) {
						if (Array.isArray(value)) {
							value.forEach((x, i) => {
								if (typeof x == 'object') formData = objToFormData(x, `${pascalKey}[${i}]`, formData)
								formData.append(`${pascalKey}[${i}]`, x.toString())
							})
						} else formData = objToFormData(value, pascalKey, formData)
					}
					break
			}
		})
	}

	return formData
}

export default objToFormData
