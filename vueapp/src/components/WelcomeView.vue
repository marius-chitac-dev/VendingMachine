<template>
    <h3>Welcome {{ getUsername() }}</h3>
</template>
  
<script lang="ts">
import { defineComponent } from "vue";
import { GET_USERNAME } from "../store/storeconstants";
export default defineComponent({
    name: 'WelcomeVIew',
    data() {
        return {
        }
    },
    methods: {
        getUsername() {
            return this.$store.getters[`auth/${GET_USERNAME}`]
        },
    }



})
</script>