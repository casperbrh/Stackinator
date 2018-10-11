using System;
using System.Collections.Generic;

namespace API
{
    public class CategoriesService
    {
        public CategoriesService()
        {
        }

        private static List<Category> database = new List<Category>();

        public static Category Read(int id)
        {
            // return single category
            for (int i = 0; i < database.Count; i++)
            {
                if (database[i].Id == id)
                {
                    return database[i];
                }
            }

            return null;
        }

        public static List<Category> Read()
        {
            List<Category> categories = new List<Category>();
            for (int i = 0; i < database.Count; i++)
            {
                categories.Add(database[i]);
            }
            return categories;
        }

        public static Category Create(string categoryName)
        {
            int nextId;
            if (database.Count == 0)
            {
                nextId = 1;
            }
            else
            {
                nextId = database[database.Count - 1].Id + 1;
            }
            Category category = new Category(nextId, categoryName);
            database.Add(category);
            return Read(category.Id);
        }
    }
}
