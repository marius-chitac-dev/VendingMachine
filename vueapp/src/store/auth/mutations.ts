import { SET_AUTHENTICATION, SET_JWT, SET_USERNAME } from "../storeconstants";
import { State } from "@vue/runtime-core";
export default {
  [SET_AUTHENTICATION](state: State, authenticated: boolean) {
    state.authenticated = authenticated;
  },
  [SET_USERNAME](state: State, username: string) {
    state.username = username;
  },
  [SET_JWT](state: State, jwt: string) {
    state.jwt = jwt;
  },
};
