<template>
    <v-toolbar app color="primary" :elevation="5">
        <v-toolbar-title>
            <router-link to="/" class="text-decoration-none white--text">
                <v-img src="@/assets/logo.png" max-height="40" max-width="40" contain class="ml-5" />
            </router-link>
        </v-toolbar-title>

        <v-spacer></v-spacer>

        <v-text-field v-model="search" append-inner-icon="mdi-magnify" label="Пошук" variant="underlined" single-line
            hide-details style="max-width: 300px;" base-color="black" clearable class="mr-5"></v-text-field>

        <v-btn icon @click="$router.push('/favorites')" class="mt-3">
            <v-icon>mdi-heart</v-icon>
        </v-btn>
        <v-btn icon @click="$router.push('/cart')" class="mt-3">
            <v-icon>mdi-cart</v-icon>
        </v-btn>

        <template v-slot:extension>
            <v-tabs v-model="currentItem" align-tabs="center" hide-slider>
                <v-menu width="500px" v-if="more.length">
                    <template v-slot:activator="{ props }">
                        <v-btn class="align-self-center me-4" variant="plain" v-bind="props" >
                            more
                        </v-btn>
                    </template>

                    <v-list class="bg-grey-lighten-3">
                        <v-list-item v-for="item in more" :key="item" :title="item"></v-list-item>
                    </v-list>
                </v-menu>
                <v-tab v-for="item in items" :key="item" :text="item"></v-tab>


            </v-tabs>
        </template>
    </v-toolbar>

</template>

<script>
    export default {
        data: () => ({
            currentItem: 'tab-Web',
            items: [
                'Web', 'Shopping', 'Videos', 'Images',
            ],
            more: [
                'News', 'Maps', 'Books', 'Flights', 'Apps',
            ],
            text: 'Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat.',
        }),

        methods: {
            addItem(item) {
                const removed = this.items.splice(0, 1)
                this.items.push(
                    ...this.more.splice(this.more.indexOf(item), 1),
                )
                this.more.push(...removed)
                this.$nextTick(() => { this.currentItem = 'tab-' + item })
            },
        },
    }
</script>

<style scoped>
    .v-text-field {
        font-size: 14px;
    }

    .v-tabs {
        flex-grow: 1;
    }

    .v-container {
        padding-top: 20px;
    }
</style>