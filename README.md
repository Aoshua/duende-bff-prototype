#Backend for Frontend

## Points Proven
- [x] BFF works
- [x] A .NET project can host our Vue application
- [x] The `SpaHost` project does not need to change when the `Api` project gets a new controller and/or endpoint
- [x] In development, we can establish hot reload for Vue application while maintaining our BFF connections

## Considerations
* We had to specifically use versions 1.1.0 (not latest) of `Duende.BFF` & `Duende.BFF.Yarp`
* Vue must run over HTTPS
* Toggle between mapped endpoints in `Program.cs` to allow for hot reload in development

## Resources
* [BFF Overview](https://docs.duendesoftware.com/identityserver/v6/bff/overview/)
* [BFF Tutorial](https://docs.duendesoftware.com/identityserver/v5/quickstarts/js_clients/js_with_backend/)
* [Tutorial (better Vue info)](https://alexpotter.dev/net-6-with-vue-3/)
* [Vite HTTPS for localhost](https://stackoverflow.com/questions/69417788/vite-https-on-localhost)
