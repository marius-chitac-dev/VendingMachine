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
        <div class="mb-3">
            <label for="seller" name="role">Seller</label>
            <input id="seller" type="radio" v-model="input.role" value="0" />
        </div>
        <div class="mb-3">
            <label for="buyer" name="role">Buyer</label>
            <input id="buyer" type="radio" v-model="input.role" value="1" />
        </div>
        <button class="btn btn-outline-dark" type="submit" v-on:click.prevent="register()">
            Register
        </button>
    </form>
</template>

<script lang="ts">
import { defineComponent } from 'vue';
import router from '../router/index';

interface Data {
    input: Input
}

interface Input {
    username: string,
    password: string,
    role: number
}

export default defineComponent({
    name: "RegisterView",
    data(): Data { return { input: { username: "", password: "", role: 0 } } },
    methods: {
        async register() {
            if (this.input.username != "" || this.input.password != "") {
                const response = await fetch('api/Users', {
                    method: "post",
                    headers: new Headers({ 'content-type': 'application/json' }),
                    body: JSON.stringify(this.input)
                });
                if (response.status === 201) {
                    router.push('/')
                }
            }
        }
    }
});
</script>