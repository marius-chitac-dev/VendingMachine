import {
  IS_USER_AUTHENTICATED,
  GET_USERNAME,
  GET_JWT,
} from "../storeconstants";
import { State } from "@vue/runtime-core";
export default {
  [IS_USER_AUTHENTICATED](state: State) {
    return state.authenticated;
  },

  [GET_USERNAME](state: State) {
    return state.username;
  },

  [GET_JWT](state: State) {
    return state.jwt;
  },
};
