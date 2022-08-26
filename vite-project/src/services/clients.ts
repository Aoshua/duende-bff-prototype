import { AnotherClient } from './apiProxy'
import { TestClient } from './apiProxy'
import { WeatherForecastClient } from './apiProxy'

export const anotherClient = new AnotherClient()
export const testClient = new TestClient()
export const weatherForecastClient = new WeatherForecastClient()
