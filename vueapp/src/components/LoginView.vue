<template>
    <form name="login-form">
        <div class="mb-3">
            <label for="username">Username: </label>
            <input id="username" type="text" v-model="input.username" />
        </div>
        <div class="mb-3">
            <label for="password">Password: </label>
            <input id="password" type="password" v-model="input.password" />
        </div>
        <button class="btn btn-outline-dark" type="submit" v-on:click.prevent="login()">
            Login
        </button>
        <button class="btn btn-outline-dark" v-on:click.prevent="register()">
            Register
        </button>
    </form>
</template>

<script lang="ts">
import { SET_AUTHENTICATION, SET_JWT, SET_USERNAME } from "../store/storeconstants";
import { defineComponent } from 'vue';
import router from '../router/index';

interface Data {
    input: Input
}

interface Input {
    username: string,
    password: string
}

export default defineComponent({
    name: "LoginView",
    data(): Data { return { input: { username: "", password: "" } } },
    methods: {
        async login() {
            if (this.input.username != "" || this.input.password != "") {
                var response = await fetch('api/Auth', {
                    method: "post",
                    headers: new Headers({ 'content-type': 'application/json' }),
                    body: JSON.stringify(this.input)
                });

                if (response.status === 200) {
                    var jwt = await response.text();
                    this.$store.commit(`auth/${SET_AUTHENTICATION}`, true);
                    this.$store.commit(`auth/${SET_USERNAME}`, this.input.username);
                    this.$store.commit(`auth/${SET_JWT}`, jwt);
                    router.push('/home');
                }
            }

        },
        register() {
            console.log("TEST");
            router.push('/register');
        }
    }
});
</script>