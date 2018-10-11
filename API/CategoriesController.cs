using System;
using System.Collections.Generic;
using Model;
using Newtonsoft.Json;

namespace API
{
    public class CategoriesController
    {
        public CategoriesController()
        {
        }

        public static void InitDatabase()
        {
            CategoriesService.Create("Beverages");
            CategoriesService.Create("Condiments");
            CategoriesService.Create("Confections");
        }
    }
}
