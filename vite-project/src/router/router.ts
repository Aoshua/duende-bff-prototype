import { createRouter, createWebHistory } from "vue-router"

const router = createRouter({
	history: createWebHistory(),
	routes: [
		{
			path: "/about",
			name: "About",
			component: () => import("../views/AboutView.vue")
		}
	]
})

export default router
