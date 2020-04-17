export default {
  data() {
    return {
      rules: {
        required: value => !!value || 'Required',
        email: value => /^(([^<>()[\]\\.,;:\s@"]+(\.[^<>()[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/
          .test(value) || 'Please enter a valid email address.',
        // from http://regexlib.com/REDetails.aspx?regexp_id=1923
        password: value => /(?=^.{8,}$)(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?!.*\s)[0-9a-zA-Z!@#$%^&*()]*$/
          .test(value) || 'Passwords must be at least 8 characters, contain a lowercase, uppercase, and number.',
      },
    }
  },
}
