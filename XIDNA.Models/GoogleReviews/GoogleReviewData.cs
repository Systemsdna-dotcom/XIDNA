using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XIDNA.Models
{
    [Table("XIPlaceReviews_T")]
    public class GoogleReviewData
    {

            [Key]
            public int ID { get; set; }
            public string sAuthorName { get; set; }
            public string sPlaceID { get; set; }
            public string sRating { get; set; }
            public string sReview { get; set; }
            public string sMonths { get; set; }
    }
}
