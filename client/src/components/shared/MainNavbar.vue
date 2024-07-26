<template>
    <div>
        <v-app-bar app color="primary" dark>
            <v-toolbar-title>
                <router-link to="/" class="text-decoration-none white--text">
                    <v-img src="@/assets/logo.png" max-height="40" max-width="40" contain class="ml-5" />
                </router-link>
            </v-toolbar-title>

            <v-spacer></v-spacer>

            <v-text-field v-model="search" append-inner-icon="mdi-magnify" label="Пошук" variant="underlined"
                single-line hide-details style="max-width: 300px;" base-color="black" clearable
                class="mr-5"></v-text-field>

            <v-btn icon @click="$router.push('/favorites')" class="mt-3">
                <v-icon>mdi-heart</v-icon>
            </v-btn>
            <v-btn icon @click="$router.push('/cart')" class="mt-3">
                <v-icon>mdi-cart</v-icon>
            </v-btn>

            <template v-slot:extension>
                <v-menu open-on-hover>
                    <template v-slot:activator="{ props }">
                        <v-tabs v-model="tab" grow hide-slider>
                            <v-tab v-bind="props">CATEGORY 1</v-tab>
                            <v-tab v-bind="props">CATEGORY 2</v-tab>
                            <v-tab v-bind="props">CATEGORY 3</v-tab>
                        </v-tabs>
                    </template>

                    <v-list>
                        <v-list-item v-if="tab === 0">
                            <v-list-item-title> SUBCATEGORY 1 for FIRST TAB</v-list-item-title>
                            <v-list-item-title> SUBCATEGORY 2 for FIRST TAB</v-list-item-title>
                            <v-list-item-title> SUBCATEGORY 3 for FIRST TAB</v-list-item-title>
                        </v-list-item>
                        <v-list-item v-if="tab === 1">
                            <v-list-item-title> SUBCATEGORY 1 for SECOND TAB</v-list-item-title>
                            <v-list-item-title> SUBCATEGORY 2 for SECOND TAB</v-list-item-title>
                            <v-list-item-title> SUBCATEGORY 3 for SECOND TAB</v-list-item-title>
                        </v-list-item>
                        <v-list-item v-if="tab === 2">
                            <v-list-item-title> SUBCATEGORY 1 for THIRD TAB</v-list-item-title>
                            <v-list-item-title> SUBCATEGORY 2 for THIRD TAB</v-list-item-title>
                            <v-list-item-title> SUBCATEGORY 3 for THIRD TAB</v-list-item-title>
                        </v-list-item>
                    </v-list>
                </v-menu>


            </template>
        </v-app-bar>
    </div>
</template>

<script>
    export default {
        name: 'MainNavbar',
        data() {
            return {
                tab: null,
                listItems: [
                    { title: 'Click Me' },
                    { title: 'Click Me' },
                    { title: 'Click Me' },
                    { title: 'Click Me 2' },
                ],
                items: [
                    'Appetizers', 'Entrees', 'Deserts', 'Cocktails',
                ],
                text: 'Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat.',
            }
        },
    }
</script>

<style scoped>
    .v-text-field {
        font-size: 14px;
    }

    .panel {
        position: absolute;
        top: 100%;
        left: 0;
        right: 0;
        background-color: white;
        box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
        z-index: 100;
    }
</style>