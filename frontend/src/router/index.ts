import { createRouter, createWebHistory } from "vue-router";
import { useAuthStore } from "../store/auth";
import Dashboard from "../views/Dashboard.vue";
import Login from "../views/Login.vue";

const routes = [
  { path: "/", component: Dashboard, meta: { requiresAuth: true } },
  { path: "/login", component: Login },
  { path: "/register", component: Login }, // Reuse login for register
];

const router = createRouter({
  history: createWebHistory(),
  routes,
});

router.beforeEach((to, _, next) => {
  const auth = useAuthStore();
  if (to.meta.requiresAuth && !auth.isAuthenticated) {
    next("/login");
  } else {
    next();
  }
});

export default router;
