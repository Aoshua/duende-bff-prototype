<script setup lang="ts">
	// This starter template is using Vue 3 <script setup> SFCs
	// Check out https://vuejs.org/api/sfc-script-setup.html#script-setup
	import { ref } from "vue"
	import HelloWorld from "./components/HelloWorld.vue"

	const userClaims = ref()

	const getUser = async () => {
		try {
			const response = await fetch(
				new Request("/bff/user", {
					// Anti-forgery header
					// https://docs.duendesoftware.com/identityserver/v6/bff/apis/local/#:~:text=mycompany.com.-,Anti%2Dforgery%20header,-In%20addition%20to
					headers: new Headers({
						"X-CSRF": "1"
					})
				})
			)
			if (response.ok) {
				userClaims.value = await response.json()
				console.log("user logged int", userClaims.value)
			} else if (response.status === 401) console.log("user not logged in")
			else console.log("user log else")
		} catch (e) {
			console.log("error checking user status")
		}
	}
	getUser()

	const login = async () => {
		window.location.href = "/bff/login"
	}

	const weather = async () => {
		console.log("getting weather...")
		try {
			const response = await fetch(
				new Request("/api/weatherForecast", {
					headers: new Headers({
						"X-CSRF": "1"
					})
				})
			)
			let data
			console.log("Request status:", response.status)
			if (response.ok) {
				data = await response.json()
				console.log("Weather result", data)
			}
		} catch (e) {
			console.log("error", e)
		}
	}

	const another = async () => {
		console.log("getting another")
		try {
			const response = await fetch(
				new Request("/api/another", {
					headers: new Headers({
						"X-CSRF": "1"
					})
				})
			)
			let data
			console.log("Request status:", response.status)
			if (response.ok) {
				data = await response.text()
				console.log("Another result", data)
			}
		} catch (e) {
			console.log("error", e)
		}
	}

	const logout = async () => {
		if (userClaims.value) {
			let logoutUrl = userClaims.value.find((claim: any) => claim.type === "bff:logout_url").value
			window.location.href = logoutUrl
		} else window.location.href = "/bff/logout"
	}
</script>

<template>
	<div>
		<a href="https://vitejs.dev" target="_blank">
			<img src="/vite.svg" class="logo" alt="Vite logo" />
		</a>
		<a href="https://vuejs.org/" target="_blank">
			<img src="./assets/vue.svg" class="logo vue" alt="Vue logo" />
		</a>
	</div>
	<HelloWorld msg="Vite + Vue" />
	<router-link :to="{ name: 'About' }">About Page</router-link>
	<RouterView></RouterView>

	<div>
		<button type="button" @click="login">Login</button>
		<button type="button" @click="weather">Get Weather</button>
		<button type="button" @click="another">Get Another</button>
		<button type="button" @click="logout">Logout</button>
	</div>
	<pre style="text-align: left">{{ userClaims }}</pre>
</template>

<style scoped>
	.logo {
		height: 6em;
		padding: 1.5em;
		will-change: filter;
	}
	.logo:hover {
		filter: drop-shadow(0 0 2em #646cffaa);
	}
	.logo.vue:hover {
		filter: drop-shadow(0 0 2em #42b883aa);
	}
</style>
