import mutations from "./mutations";
import getters from "./getters";
import { State } from "@vue/runtime-core";

export default {
  namespaced: true,
  state(): State {
    return {
      authenticated: false,
      username: "",
      jwt: "",
    };
  },
  mutations,
  getters,
};
