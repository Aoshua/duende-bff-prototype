<script setup lang="ts">
	import { testClient } from '../services/clients'

	const test = async () => {
		const r = await testClient.test()
		console.log(r)
	}
</script>

<template>
	<h1 @click="test">About</h1>
</template>

<style scoped lang="less"></style>
