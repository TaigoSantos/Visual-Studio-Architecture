﻿using System;
using System.Data.Entity;
using System.Web.Http;
using AutoMapper;
using Microsoft.AspNet.Identity;
using Owin;
using Project.API.Base.MapperAdapters;
using Project.API.Core.Mappers;
using Project.Configurations;
using Project.Domain.Core.Domains;
using Project.Domain.Core.Interfaces;
using Project.Models.Core.Entities;
using Project.Persistence.Core.Contexts;
using Project.Persistence.Core.Contexts.Base;
using Project.Persistence.Core.Interfaces;
using Project.Persistence.Core.Repositories;
using SimpleInjector;
using SimpleInjector.Extensions.ExecutionContextScoping;
using SimpleInjector.Integration.WebApi;

namespace Project.API.Core
{
    public class DependecyConfig
    {
        public static Container Container { get; set; }

        public static void ConfigureDependencies(HttpConfiguration config, IAppBuilder app)
        {
            Container = new Container();
            Container.Options.DefaultScopedLifestyle = new ExecutionContextScopeLifestyle();
            Container.RegisterWebApiControllers(GlobalConfiguration.Configuration);

            ConfigContainer(Container);
            Container.Verify();

            app.Use(async (context, next) =>
            {
                using (Container.BeginExecutionContextScope())
                {
                    await next();
                }
            });

            config.DependencyResolver = new SimpleInjectorWebApiDependencyResolver(Container);
        }

        public static void ConfigContainer(Container container)
        {
            #region - MAPPER -
            var mapperConfig = AutoMapperConfig.GetMapperConfiguration();
            container.RegisterSingleton<IConfigurationProvider>(mapperConfig);
            container.Register(() => mapperConfig.CreateMapper());
            container.Register<IMapperAdapter, AutoMapperAdapter>();
            #endregion

            #region - PERSISTENCE -

            if (GlobalSettings.DATABASE == DatabaseType.SqlServer)
            {
                // context
                container.Register<DbContext, CoreContext>(Lifestyle.Scoped);

                // repositories
                container.Register<IClientRepository, ClientRepository>(Lifestyle.Scoped);
                container.Register<IRefreshTokenRepository, RefreshTokenRepository>(Lifestyle.Scoped);
                container.Register<IUsuarioRepository<Guid>, UsuarioRepository>(Lifestyle.Scoped);
            }
            else if (GlobalSettings.DATABASE == DatabaseType.Mongo)
            {
                // context
                container.Register<MongoContextBase, CoreMongoContext>(Lifestyle.Scoped);

                // repositories
                container.Register<IClientRepository, ClientMongoRepository>(Lifestyle.Scoped);
                container.Register<IRefreshTokenRepository, RefreshTokenMongoRepository>(Lifestyle.Scoped);
                container.Register<IUsuarioRepository<Guid>, UsuarioMongoRepository>(Lifestyle.Scoped);
            }

            #endregion

            #region - DOMAINS -

            var registration = Lifestyle.Scoped.CreateRegistration<UsuarioDomain>(container);
            container.AddRegistration(typeof(IUsuarioDomain), registration);
            container.AddRegistration(typeof(IUserStore<Usuario, Guid>), registration);

            container.Register<UserManager<Usuario, Guid>>(Lifestyle.Scoped);
            container.Register<IRefreshTokenDomain, RefreshTokenDomain>(Lifestyle.Scoped);
            container.Register<IClientDomain, ClientDomain>(Lifestyle.Scoped);
            container.Register<IAccountDomain<Guid>, AccountDomain>(Lifestyle.Scoped);

            #endregion
        }
    }
}
