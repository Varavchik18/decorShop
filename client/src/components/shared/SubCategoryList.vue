<template>
    <v-container fluid>
        <v-card class="mx-auto" >
            <v-list >
                <v-list-subheader>REPORTS</v-list-subheader>

                <v-list-item v-for="(item, i) in items" :key="i" :value="item" color="primary">
                    <template v-slot:prepend>
                        <v-icon :icon="item.icon"></v-icon>
                    </template>

                    <v-list-item-title v-text="item.text"></v-list-item-title>
                </v-list-item>
            </v-list>
        </v-card>
    </v-container>
</template>

<script>
    export default {
        name: 'SubCategoryList',
        data: () => ({
            items: [
                { text: 'Real-Time', icon: 'mdi-clock' },
                { text: 'Audience', icon: 'mdi-account' },
                { text: 'Conversions', icon: 'mdi-flag' },
            ],
        }),
    };
</script>