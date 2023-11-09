using Autofac;
using Common;
using Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Reflection;
using System.Text.Json.Serialization;
using WebFramework.Configuration;
using WebFramework.Cors;
using WebFramework.CustomMapping;
using WebFramework.Middlewares;
using WebFramework.Swagger;
using Entities.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Admin
{
    public class Startup
    {
        private readonly SiteSettings _siteSetting;
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            _siteSetting = configuration.GetSection(nameof(SiteSettings)).Get<SiteSettings>();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            PopulateControllersAndActions();
            PopulatePermissionsAndGroupsIntoDatabase();
            services.Configure<SiteSettings>(Configuration.GetSection(nameof(SiteSettings)));
            services.AddControllers().AddJsonOptions(x =>
             x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve);

            services.InitializeAutoMapper();

            services.AddDbContext(Configuration);

            services.AddCustomIdentity(_siteSetting.IdentitySettings);

            //services.AddMinimalMvc();
            services.AddMvc(options => options.EnableEndpointRouting = false);

            services.AddElmahCore(Configuration, _siteSetting);

            services.AddJwtAuthentication(_siteSetting.JwtSettings);

            services.AddCustomApiVersioning();

            var projectName = Assembly.GetEntryAssembly().GetName().Name;
            services.AddSwagger(projectName);
            services.AddCustomeAdminCors();

            // Don't create a ContainerBuilder for Autofac here, and don't call builder.Populate()
            // That happens in the AutofacServiceProviderFactory for you.            
        }
        List<ControllerData> PopulateControllersAndActions()
        {
            List<ControllerData> data = new List<ControllerData>();

            IEnumerable<Assembly> assemblies = AppDomain.CurrentDomain.GetAssemblies()
                // Ignore assemblies which their name start with following names:
                .Where(a => !a.FullName.StartsWith("mscorlib")
                            && !a.FullName.StartsWith("System")
                            && !a.FullName.StartsWith("Microsoft")
                            && !a.FullName.StartsWith("WebDev")
                            && !a.FullName.StartsWith("SMDiagnostics")
                            && !a.FullName.StartsWith("Anonymously")
                            && !a.FullName.StartsWith("Antlr3")
                            && !a.FullName.StartsWith("EntityFramework")
                            && !a.FullName.StartsWith("Newtonsoft")
                            && !a.FullName.StartsWith("Owin")
                            && !a.FullName.StartsWith("WebGrease")
                            && !a.FullName.StartsWith("App_")
                            && !a.FullName.StartsWith("Kendo"));

            foreach (Assembly assembly in assemblies)
            {
                // http://stackoverflow.com/questions/1423733/how-to-tell-if-a-net-assembly-is-dynamic
                if (!(assembly.ManifestModule is System.Reflection.Emit.ModuleBuilder)
                    && assembly.ManifestModule.GetType().Namespace != "System.Reflection.Emit")
                {
                    Type[] types = assembly.GetTypes().Where(t => t.BaseType == typeof(Controller)).ToArray();

                    foreach (var type in types)
                    {
                        bool controllerHasAuthorizeAttribute =
                            type.CustomAttributes.Any(c => c.AttributeType.Name == "AuthorizeAttribute");

                        bool classHasIgnorePermissionCheck =
                            type.CustomAttributes.Any(c => c.AttributeType.Name == "IgnorePermissionCheckAttribute");
                        if (classHasIgnorePermissionCheck) // if the class has this attribute, that means it's marked to ignore permission check
                        {
                            continue;
                        }

                        string controllerNameLocalized = null;
                        var classTitle = type.CustomAttributes.FirstOrDefault(c => c.AttributeType.Name == "TitleAttribute");
                        if (classTitle != null)
                        {
                            controllerNameLocalized = classTitle.ConstructorArguments[0].Value.ToString();
                        }

                        var controllerData = new ControllerData()
                        {
                            ControllerNamespace = type.Namespace,
                            ControllerName = type.Name.Replace("Controller", ""),
                            RequiresAuthorization = controllerHasAuthorizeAttribute,
                            ControllerNameLocalized = controllerNameLocalized
                        };

                        var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance);
                        List<string> actions = new List<string>();
                        foreach (MethodInfo methodInfo in methods)
                        {
                            // Check if method has IgnorePermissionCheck attribute
                            bool methodHasIgnorePermissionCheck = methodInfo.CustomAttributes
                                    .Any(c => c.AttributeType.Name == "IgnorePermissionCheckAttribute");
                            if (methodHasIgnorePermissionCheck)
                            {
                                continue;
                            }

                            // Check if method has ChildActionOnlyAttribute
                            bool methodHasChildActionOnlyAttribute = methodInfo.CustomAttributes
                                .Any(c => c.AttributeType.Name == "ChildActionOnlyAttribute");
                            if (methodHasChildActionOnlyAttribute)
                            {
                                continue;
                            }

                            // Check for other attributes important to us
                            bool isAllowAnonymous = methodInfo.CustomAttributes.Any(c => c.AttributeType.Name == "AllowAnonymousAttribute");
                            var httpPostAttribute = methodInfo.CustomAttributes.FirstOrDefault(c => c.AttributeType.Name == "HttpPostAttribute");
                            var actionNameAttribute = methodInfo.CustomAttributes.FirstOrDefault(c => c.AttributeType.Name == "ActionNameAttribute");
                            bool actionRequiresAuthorization = methodInfo.CustomAttributes.Any(c => c.AttributeType.Name == "AuthorizeAttribute");
                            bool isHttpPost = httpPostAttribute != null;


                            // ActionNameAttribute could rename an action, if it's used, then use new name of action
                            string actionName = null;
                            if (actionNameAttribute != null)
                            {
                                actionName = actionNameAttribute.ConstructorArguments[0].Value.ToString();
                            }


                            string actionNameLocalized = null;
                            var actionTitle = methodInfo.CustomAttributes.FirstOrDefault(c => c.AttributeType.Name == "TitleAttribute");
                            if (actionTitle != null) // if the class has this attribute, that means it's marked to ignore permission check
                            {
                                actionNameLocalized = actionTitle.ConstructorArguments[0].Value.ToString();
                            }

                            string actionIcon = null;
                            var displayIcon = methodInfo.CustomAttributes.FirstOrDefault(c => c.AttributeType.Name == "IconAttribute");
                            if (displayIcon != null) // if the class has this attribute, that means it's marked to ignore permission check
                            {
                                actionIcon = displayIcon.ConstructorArguments[0].Value.ToString();
                            }

                            // Gather only methods with ActionResult return types:
                            if (methodInfo.ReturnType.Name == "ActionResult")
                            {
                                //actions.Add(methodInfo.Name);
                                if (!controllerData.ActionsList.Any(cd => cd.ActionName == (actionName ?? methodInfo.Name)))
                                {
                                    controllerData.ActionsList.Add(new ActionData()
                                    {
                                        ActionName = actionName ?? methodInfo.Name,
                                        ActionNameLocalized = actionNameLocalized,
                                        ActionIcon = actionIcon,
                                        AllowAnonymous = isAllowAnonymous,
                                        RequiresHttpPost = isHttpPost,
                                        RequiresAuthorization = actionRequiresAuthorization
                                    });
                                }
                            }
                        }
                        data.Add(controllerData);
                    }
                }
            }
            return data;
        }
        void PopulatePermissionsAndGroupsIntoDatabase()
        {

            ApplicationDbContext db = new ApplicationDbContext();
            var data = PopulateControllersAndActions();

            var exisitingGroups = db.PermissionGroups.Include("Permissions").ToList();

            foreach (var controllerData in data)
            {
                var group = exisitingGroups
                    .FirstOrDefault(g =>
                            g.PermissionGroupTitle == controllerData.ControllerName
                            && g.PermissionGroupNamespace == controllerData.ControllerNamespace);

                if (group != null)
                {
                    foreach (var action in controllerData.ActionsList)
                    {
                        string actionFullName = $"{controllerData.ControllerName}{action.ActionName}";

                        var currentPermission =
                                    group.Permissions.FirstOrDefault(p => p.ActionFullName == actionFullName);

                        if (currentPermission == null)
                        {
                            var permission = new Permission()
                            {
                                PermissionGroupID = group.Id,
                                PermissionTitle = action.ActionName,
                                PermissionTitleLocalized = action.ActionNameLocalized,
                                PermissionIcon = action.ActionIcon,
                                RequiresAuthorization = action.RequiresAuthorization,
                                AllowAnonymous = action.AllowAnonymous,
                                ActionFullName = actionFullName
                            };
                            group.Permissions.Add(permission);
                        }
                        else
                        {
                            if (currentPermission.AllowAnonymous != action.AllowAnonymous || currentPermission.RequiresAuthorization != (action.RequiresAuthorization))
                            {
                                currentPermission.AllowAnonymous = action.AllowAnonymous;
                                currentPermission.RequiresAuthorization = action.RequiresAuthorization;
                            }
                            if (group.RequiresAuthorization != controllerData.RequiresAuthorization)
                            {
                                group.RequiresAuthorization = controllerData.RequiresAuthorization;
                            }

                            if (!string.IsNullOrEmpty(action.ActionNameLocalized) && currentPermission.PermissionTitleLocalized != action.ActionNameLocalized)
                            {
                                currentPermission.PermissionTitleLocalized = action.ActionNameLocalized;
                            }

                            if (!string.IsNullOrEmpty(action.ActionIcon) && currentPermission.PermissionIcon != action.ActionIcon)
                            {
                                currentPermission.PermissionIcon = action.ActionIcon;
                            }

                            if (!string.IsNullOrEmpty(controllerData.ControllerNameLocalized) && group.PermissionGroupTitleLocalized != controllerData.ControllerNameLocalized)
                            {
                                group.PermissionGroupTitleLocalized = controllerData.ControllerNameLocalized;
                            }

                            db.SaveChanges();

                        }
                    }
                }
                else
                {
                    var newGroup = new PermissionGroup()
                    {
                        PermissionGroupTitle = controllerData.ControllerName,
                        PermissionGroupTitleLocalized = controllerData.ControllerNameLocalized,
                        PermissionGroupNamespace = controllerData.ControllerNamespace,
                        RequiresAuthorization = controllerData.RequiresAuthorization
                    };

                    foreach (var actionData in controllerData.ActionsList)
                    {
                        string actionName = $"{controllerData.ControllerName}{actionData.ActionName}";
                        var permission = new Permission()
                        {
                            PermissionGroupID = newGroup.Id,
                            PermissionTitle = actionData.ActionName,
                            PermissionTitleLocalized = actionData.ActionNameLocalized,
                            PermissionIcon = actionData.ActionIcon,
                            RequiresAuthorization = actionData.RequiresAuthorization,
                            AllowAnonymous = actionData.AllowAnonymous,
                            ActionFullName = actionName
                        };
                        if (permission.RequiresAuthorization == false && permission.AllowAnonymous == false && newGroup.RequiresAuthorization == true)
                        {
                            permission.RequiresAuthorization = true;
                        }
                        newGroup.Permissions.Add(permission);
                    }
                    db.PermissionGroups.Add(newGroup);
                }
            }
            db.SaveChanges();

        }
        // ConfigureContainer is where you can register things directly with Autofac. 
        // This runs after ConfigureServices so the things ere will override registrations made in ConfigureServices.
        // Don't build the container; that gets done for you by the factory.
        public void ConfigureContainer(ContainerBuilder builder)
        {
            //Register Services to Autofac ContainerBuilder
            builder.AddServices();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.IntializeDatabase();

            app.UseCustomExceptionHandler();

            app.UseHsts(env);

            app.UseHttpsRedirection();

            app.UseElmahCore(_siteSetting);

            app.UseSwaggerAndUI();

            app.UseRouting();

            app.UseMvc();
            app.UseCors();

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseStaticFiles();

            //Use this config just in Develoment (not in Production)
            //app.UseCors(config => config.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());

            app.UseEndpoints(config =>
            {
                config.MapControllers(); // Map attribute routing
                //    .RequireAuthorization(); Apply AuthorizeFilter as global filter to all endpoints
                //config.MapDefaultControllerRoute(); // Map default route {controller=Home}/{action=Index}/{id?}
            });

            //Using 'UseMvc' to configure MVC is not supported while using Endpoint Routing.
            //To continue using 'UseMvc', please set 'MvcOptions.EnableEndpointRouting = false' inside 'ConfigureServices'.
            //app.UseMvc();
        }
    }
}
