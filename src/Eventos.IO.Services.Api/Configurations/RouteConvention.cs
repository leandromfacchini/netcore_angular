﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Routing;
using System;
using System.Linq;

namespace Eventos.IO.Services.Api.Configurations
{
    public class RouteConvention : IApplicationModelConvention
    {
        private readonly AttributeRouteModel _centralPrefix;

        public RouteConvention(IRouteTemplateProvider routeTemplateProvider)
        {
            _centralPrefix = new AttributeRouteModel(routeTemplateProvider);
        }

        public void Apply(ApplicationModel application)
        {
            foreach (var controller in application.Controllers)
            {
                var matchedSelectors = controller.Selectors.Where(c => c.AttributeRouteModel != null).ToList();
                if (matchedSelectors.Any())
                {
                    foreach (var selectorModel in matchedSelectors)
                    {
                        selectorModel.AttributeRouteModel = AttributeRouteModel.CombineAttributeRouteModel(_centralPrefix, selectorModel.AttributeRouteModel);
                    }
                }

                var unmatchedSelector = controller.Selectors.Where(c => c.AttributeRouteModel == null).ToList();
                if (unmatchedSelector.Any())
                {
                    foreach (var selectorModel in unmatchedSelector)
                    {
                        selectorModel.AttributeRouteModel = _centralPrefix;

                    }
                }
            }
        }
    }

    public static class MvcOptionsExtensions
    {
        public static void UseCentralRoutePrefix(this MvcOptions options, IRouteTemplateProvider routeAttribute)
        {
            options.Conventions.Insert(0, new RouteConvention(routeAttribute));
        }
    }

}
