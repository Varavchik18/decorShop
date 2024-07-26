import { createVuetify } from 'vuetify'
import { VApp, VAppBar, VAppBarNavIcon, VBtn, VIcon } from 'vuetify/components'
import { aliases, mdi } from 'vuetify/iconsets/mdi'
import 'vuetify/styles'
import '@mdi/font/css/materialdesignicons.css' 
import colors from 'vuetify/util/colors'

export default createVuetify({
  components: {
    VApp,
    VAppBar,
    VAppBarNavIcon,
    VBtn,
    VIcon,
  },
  icons: {
    defaultSet: 'mdi',
    aliases,
    sets: {
      mdi,
    },
  },
  theme: {
    defaultTheme: 'light',
    themes: {
      light: {
        dark: false,
        colors: {
          primary: colors.grey.lighten3, 
          secondary: colors.grey.darken4, 
          info: colors.lightBlue.lighten2,
          warning: colors.deepOrange.darken1,
          error: colors.red.darken2
        }
      }
    }
  }
})