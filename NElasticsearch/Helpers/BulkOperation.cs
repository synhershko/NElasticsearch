using System.Collections.Generic;
using NElasticsearch.Models;

namespace NElasticsearch.Helpers
{
    public class BulkOperation
    {
        public List<BulkOperationItem> BulkOperationItems { get; set; }

        public BulkOperation()
        {
            BulkOperationItems = new List<BulkOperationItem>();
        }

        public static BulkOperationOnKnownIndex On(string index)
        {
            return new BulkOperationOnKnownIndex(index);
        }

        public static BulkOperationOnKnownIndexAndType On(string index, string type)
        {
            return new BulkOperationOnKnownIndexAndType(index, type);
        }

        public BulkOperation Index(string index, string type, object obj, string id = null)
        {
            BulkOperationItems.Add(new BulkOperationItem.Index(index, type, obj, id));
            return this;
        }

        public BulkOperation Create(string index, string type, object obj, string id)
        {
            BulkOperationItems.Add(new BulkOperationItem.Create(index, type, obj, id));
            return this;
        }

        public BulkOperation Delete(string index, string type, string id)
        {
            BulkOperationItems.Add(new BulkOperationItem.Delete(index, type, id));
            return this;
        }
    }

    public class BulkOperationOnKnownIndex : BulkOperation
    {
        protected readonly string IndexName;

        public BulkOperationOnKnownIndex(string indexName)
        {
            IndexName = indexName;
        }

        public BulkOperationOnKnownIndex Index(string type, object obj, string id = null)
        {
            BulkOperationItems.Add(new BulkOperationItem.Index(IndexName, type, obj, id));
            return this;
        }

        public BulkOperationOnKnownIndex Create(string type, object obj, string id)
        {
            BulkOperationItems.Add(new BulkOperationItem.Create(IndexName, type, obj, id));
            return this;
        }

        public BulkOperationOnKnownIndex Delete(string type, string id)
        {
            BulkOperationItems.Add(new BulkOperationItem.Delete(IndexName, type, id));
            return this;
        }
    }

    public class BulkOperationOnKnownIndexAndType : BulkOperationOnKnownIndex
    {
        protected readonly string TypeName;

        public BulkOperationOnKnownIndexAndType(string index, string type) : base(index)
        {
            TypeName = type;
        }

        public BulkOperationOnKnownIndexAndType Index(object obj, string id = null)
        {
            BulkOperationItems.Add(new BulkOperationItem.Index(IndexName, TypeName, obj, id));
            return this;
        }

        public BulkOperationOnKnownIndexAndType Create(object obj, string id)
        {
            BulkOperationItems.Add(new BulkOperationItem.Create(IndexName, TypeName, obj, id));
            return this;
        }

        public BulkOperationOnKnownIndexAndType Delete(string id)
        {
            BulkOperationItems.Add(new BulkOperationItem.Delete(IndexName, TypeName, id));
            return this;
        }
    }
}
