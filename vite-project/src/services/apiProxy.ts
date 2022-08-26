import http from './http'
import objToFormData from './formData'

export class AnotherClient {
	async another(): Promise<string> {
		let url_ = '/Another?'
		url_ = url_.replace(/[?&]$/, '')

		const response = await http.request({
			method: 'GET',
			url: url_,
			headers: {
				'Accept': 'text/plain',
				'X-CSRF': '1'
			}
		})

		return response.data
	}
}

export class TestClient {
	async test(): Promise<string> {
		let url_ = '/Test?'
		url_ = url_.replace(/[?&]$/, '')

		const response = await http.request({
			method: 'GET',
			url: url_,
			headers: {
				'Accept': 'text/plain',
				'X-CSRF': '1'
			}
		})

		return response.data
	}
}

export class WeatherForecastClient {
	async weatherForecast(): Promise<WeatherForecast[]> {
		let url_ = '/WeatherForecast?'
		url_ = url_.replace(/[?&]$/, '')

		const response = await http.request({
			method: 'GET',
			url: url_,
			headers: {
				'Accept': 'text/plain',
				'X-CSRF': '1'
			}
		})

		const list: WeatherForecast[] = []
		const responseList = response.data
		if (Array.isArray(responseList)) {
			for (let item of responseList) list.push(new WeatherForecast(item))
		}
		return list
	}
}

export interface IWeatherForecast {
	[x: string]: any
	date?: Date
	temperatureC?: number
	temperatureF?: number
	summary?: string
}
export class WeatherForecast {
	[x: string]: any
	date?: Date
	temperatureC?: number
	temperatureF?: number
	summary?: string

	constructor(data?: IWeatherForecast) {
		if (data) {
			this.date = data.date
			this.temperatureC = data.temperatureC
			this.temperatureF = data.temperatureF
			this.summary = data.summary
		}
	}

	static newEmpty(data?: { [x: string]: any, date?: Date, temperatureC?: number, temperatureF?: number, summary?: string }) {
		let empty: IWeatherForecast = {}
		if (data != null)
			Object.keys(data).forEach(key => { if (data[key] != null) empty[key] = data[key] })

		return new WeatherForecast(empty)
	}
}

export interface IProxyFileParameter {
	data: any
	fileName: string
}
