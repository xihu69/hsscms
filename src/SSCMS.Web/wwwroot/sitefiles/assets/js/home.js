var ACCESS_TOKEN_NAME = "ss_user_access_token";
var $apiUrl = "/api/home";
var $rootUrl = "/home";
var URL_TOKEN_NAME ="access_token"

if (hcom)
hcom.$api = axios.create({
  //baseURL: $apiUrl,
  headers: {
    Authorization: "Bearer " + hcom.$token
  },
});
