// Redirects requests from subdomains to primary domain
// and redirects requests to index.html
function handler(event) {
    let request = event.request;
    let host = request.headers.host;
    let primaryDomain = `https://tetris.favianflores.com${request.uri}`

    if (request.uri !== "/" && (request.uri.endsWith("/") || request.uri.lastIndexOf(".") < request.uri.lastIndexOf("/"))) {
        if (request.uri.endsWith("/")) {
            request.uri = request.uri.concat("index.html");
        } else {
            request.uri = request.uri.concat("/index.html");
        }
    }

    return request;
}
