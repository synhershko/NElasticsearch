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

        public static BulkOperationOnKnownIndex On(string index, string type)
        {
            return new BulkOperationOnKnownIndexAndType(index, type);
        }

        public BulkOperationItem.Index Index(string index, string type, object obj, string id = null)
        {
            return new BulkOperationItem.Index(index, type, obj, id);
        }

        public BulkOperationItem.Create Create(string index, string type, object obj, string id)
        {
            return new BulkOperationItem.Create(index, type, obj, id);
        }

        public BulkOperationItem.Delete Delete(string index, string type, string id)
        {
            return new BulkOperationItem.Delete(index, type, id);
        }
    }

    public class BulkOperationOnKnownIndex : BulkOperation
    {
        protected readonly string IndexName;

        public BulkOperationOnKnownIndex(string indexName)
        {
            IndexName = indexName;
        }

        public BulkOperationItem.Index Index(string type, object obj, string id = null)
        {
            return new BulkOperationItem.Index(IndexName, type, obj, id);
        }

        public BulkOperationItem.Create Create(string type, object obj, string id)
        {
            return new BulkOperationItem.Create(IndexName, type, obj, id);
        }

        public BulkOperationItem.Delete Delete(string type, string id)
        {
            return new BulkOperationItem.Delete(IndexName, type, id);
        }
    }

    public class BulkOperationOnKnownIndexAndType : BulkOperationOnKnownIndex
    {
        protected readonly string TypeName;

        public BulkOperationOnKnownIndexAndType(string index, string type) : base(index)
        {
            TypeName = type;
        }

        public BulkOperationItem.Index Index(object obj, string id = null)
        {
            return new BulkOperationItem.Index(IndexName, TypeName, obj, id);
        }

        public BulkOperationItem.Create Create(object obj, string id)
        {
            return new BulkOperationItem.Create(IndexName, TypeName, obj, id);
        }

        public BulkOperationItem.Delete Delete(string id)
        {
            return new BulkOperationItem.Delete(IndexName, TypeName, id);
        }
    }
}
