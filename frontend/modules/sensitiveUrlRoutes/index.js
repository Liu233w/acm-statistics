import { defineNuxtModule, extendPages } from '@nuxt/kit'

const sensitiveRoutes = [
  '/index.html',
  '/index.php',
  '/index.jsp',
  '/admin/',
  '/wp-login.php',
  '/readme.html',
  '/license.txt',
  '/wp-includes/js/wplink.js',
  '/wp-admin/js/customize-controls.js',
  '/wp-admin/js/nav-menu.js',
  '/wp-includes/js/plupload',
  '/wp-includes/js/tinymce/',
  '/README',
  '/phpMyAdmin/',
  '/phpmyadmin/',
  '/pma/',
  '/solr/',
  '/wcm/',
  '/swagger/elpsycongroo',
  '/ZeroClipboard.swf',
  '/js/ZeroClipboard.swf',
  '/script/ZeroClipboard.swf',
  '/lib/ZeroClipboard.swf',
  '/api.php',
  '/checktable.php',
  '/theme/default/images/kindeditor/save.gif',
  '/js/kindeditor/Makefile',
  '/theme/default/images/treeview/file.gif',
  '/js/jquery/treeview/min.js',
  '/theme/default/images/main/logo.png',
  '/js/jquery/syntaxhighlighter/scripts/shBrushPlain.js',
  '/theme/default/style.css',
  '/js/my.full.js',
  '/theme/zui/fonts/zenicon.eot',
  '/README.md',
]

export default defineNuxtModule({
  setup() {
    extendPages(({ routes }) => {
      for (let route of sensitiveRoutes) {
        routes.push({
          path: route,
          component: '~/pages/jojo.vue',
        })
      }
    })
  },
})
