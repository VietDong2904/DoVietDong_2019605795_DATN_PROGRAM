using System;

namespace Infrastructure.Persistence.Businesses.ProductReview
{
    public class ProductReviewBaseModel
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public string ProductName { get; set; }
        public string UserName { get; set; }
        public string Code { get; set; }
        public Guid? ParentId { get; set; }
        public string ParentCode { get; set; }
        public Guid UserId { get; set; }
        public bool IsRating { get; set; }
        public string AvatarUrl { get; set; }
        public int Rating { get; set; }
        public bool Status { get; set; }
        public string Content { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
    }

    public class ProductReviewModel : ProductReviewBaseModel
    {
        public int Order { get; set; } = 0;

    }

    public class ProductReviewDetailModel : ProductReviewModel
    {
        public Guid? CreatedUserId { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public Guid? ModifiedUserId { get; set; }
    }

    public class ProductReviewCreateModel : ProductReviewModel
    {
        public Guid? CreatedUserId { get; set; }
        public Guid? ApplicationId { get; set; }
    }

    public class ProductReviewUpdateModel : ProductReviewModel
    {
        public Guid ModifiedUserId { get; set; }

        public void UpdateToEntity(Domain.Entities.ProductReview entity)
        {
            entity.ProductId = ProductId;
            entity.Rating = Rating;
            entity.Status = Status;
            entity.Content =Content;
            entity.ParentId = ParentId;
        }
    }

    public class ProductReviewQueryFilter
    {
        public string TextSearch { get; set; }
        public int? PageSize { get; set; }
        public Guid? ParentId { get; set; }
        public Guid? ProductId { get; set; }
        public int? PageNumber { get; set; }
        public bool? Status { get; set; }
        public string PropertyName { get; set; } = "CreatedDate";
        //asc - desc
        public string Ascending { get; set; } = "desc";
        public ProductReviewQueryFilter()
        {
            PageNumber = 1;
            PageSize = 20;
        }
    }
}