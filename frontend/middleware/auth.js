/**
 * 用来管理页面访问的中间件
 * @param {Context} param0 nuxt context
 */
export default function ({ route, redirect, store }) {

  switch (route.path) {

    case '/login':
    case '/register':
      if (store.state.session.login) {
        redirect('/settings')
      }
      break

    case '/settings':
      if (!store.state.session.login) {
        redirect('/login')
      }
      break
  }

}
