import { createWebHistory, createRouter } from "vue-router";
import store from "../store";
import { IS_USER_AUTHENTICATED } from "../store/storeconstants";

const routes = [
  {
    path: "/",
    name: "Login",
    component: () => import("../components/LoginView.vue"),
  },
  {
    path: "/register",
    name: "Register",
    component: () => import("../components/RegisterView.vue"),
  },
  {
    path: "/home",
    component: () => import("../components/WelcomeView.vue"),
  },
];

const router = createRouter({
  history: createWebHistory(process.env.BASE_URL),
  routes: routes,
  linkActiveClass: "active",
});

router.beforeEach(async (to, from) => {
  const authenticated = store.getters[`auth/${IS_USER_AUTHENTICATED}`];

  if (!authenticated && to.name !== "Login" && to.name !== "Register") {
    return { name: "Login" };
  }
});

export default router;
