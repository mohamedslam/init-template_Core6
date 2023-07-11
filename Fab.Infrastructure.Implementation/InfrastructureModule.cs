using Autofac;
using Autofac.Core;
using Fab.Infrastructure.Implementation.Authentication;
using Fab.Infrastructure.Implementation.Notifications.Transports;
using Fab.Infrastructure.Implementation.Sms.SmsRu;
using Fab.Infrastructure.Implementation.Sms.Zanzara;
using Fab.Infrastructure.Implementation.Sms.Zanzara.Soap;
using Fab.Infrastructure.Interfaces.Notifications;
using Fab.Infrastructure.Interfaces.Push;
using Fab.Infrastructure.Interfaces.Resources;
using Fab.Infrastructure.Interfaces.Sms;
using Fab.Utils.Extensions;
using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using Microsoft.Extensions.Options;
using Minio;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.ServiceModel;
using static Fab.Infrastructure.Implementation.Sms.Zanzara.Soap.ZanzaraSMSServiceSoapClient.EndpointConfiguration;
using Module = Autofac.Module;

namespace Fab.Infrastructure.Implementation;

public class InfrastructureModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterAssemblyTypes(GetType().GetTypeInfo().Assembly)
               .PublicOnly()
               .Except<IStartable>()
               .Except<INotificationTransport>()
               .AsImplementedInterfaces()
               .AsSelf()
               .SingleInstance();

        builder.Register(_ => new JwtSecurityTokenHandler())
               .SingleInstance();

        builder.RegisterType<AuthenticationService>()
               .AsImplementedInterfaces()
               .InstancePerLifetimeScope();

        builder.RegisterType<SmsRuService>()
               .Named<ISmsService>("SmsRu")
               .WithParameter(new ResolvedParameter(
                   (pi, _) => pi.ParameterType == typeof(HttpClient),
                   (_, ctx) => ctx.Resolve<IHttpClientFactory>()
                                  .CreateClient()
                                  .Also(x => x.BaseAddress = new Uri("https://sms.ru"))));

        builder.Register(ctx =>
                   new ZanzaraSMSServiceSoapClient(ZanzaraSMSServiceSoap,
                       new EndpointAddress(ctx.Resolve<IOptions<ZanzaraOptions>>().Value.Endpoint)))
               .AsImplementedInterfaces();

        builder.RegisterType<ZanzaraSmsService>()
               .Named<ISmsService>("Zanzara");


        builder.RegisterType<EmailTransport>()
               .Keyed<INotificationTransport>(NotificationChannel.Email);

        builder.RegisterType<SmsTransport>()
               .Keyed<INotificationTransport>(NotificationChannel.Sms)
               .WithParameter(new ResolvedParameter(
                   (pi, _) => pi.ParameterType == typeof(ISmsService),
                   (_, ctx) => ctx.ResolveNamed<ISmsService>("Zanzara")));

        builder.RegisterType<PushTransport>()
               .Keyed<INotificationTransport>(NotificationChannel.Push);

        builder.RegisterType<StorageTransport>()
               .Keyed<INotificationTransport>(NotificationChannel.Storage);


        builder.Register(ctx =>
               {
                   var options = ctx.Resolve<IOptions<S3Options>>().Value;
        
                   if (!Uri.TryCreate(options.Endpoint, UriKind.Absolute, out var uri))
                   {
                       throw new ArgumentException("Invalid s3 endpoint");
                   }
        
                   if (string.IsNullOrWhiteSpace(options.Bucket))
                   {
                       throw new ArgumentNullException(nameof(options.Bucket), "S3 bucket is not defined");
                   }
        
                   return new MinioClient().WithEndpoint(uri.Host)
                                           .WithCredentials(options.AccessKey, options.SecretKey)
                                           .WithRegion(options.Region)
                                           .WithSSL()
                                           .Build();
               })
               .AsSelf()
               .AsImplementedInterfaces()
               .SingleInstance();
        
        builder.Register(ctx => FirebaseApp.Create(new AppOptions
               {
                   Credential =
                       GoogleCredential.FromServiceAccountCredential(
                                           new ServiceAccountCredential(
                                               ctx.Resolve<IOptions<PushOptions>>().Value
                                                  .Let(opts =>
                                                      new ServiceAccountCredential.Initializer(opts.ClientEmail)
                                                          {
                                                              ProjectId = opts.ProjectId,
                                                              KeyId = opts.PrivateKeyId
                                                          }
                                                          .FromPrivateKey(opts.PrivateKey))))
                                       .CreateScoped("https://www.googleapis.com/auth/firebase.messaging")
               }))
               .SingleInstance();

        builder.Register(ctx => FirebaseMessaging.GetMessaging(ctx.Resolve<FirebaseApp>()))
               .SingleInstance();
    }
}