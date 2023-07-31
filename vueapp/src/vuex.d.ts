import { Store } from "vuex";

declare module "@vue/runtime-core" {
  interface State {
    authenticated: boolean;
    username: string;
    jwt: string;
  }

  interface ComponentCustomProperties {
    $store: Store<State>;
  }
}
