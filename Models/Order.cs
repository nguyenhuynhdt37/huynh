using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using OnlineCourse.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace huynh.Models
{
    public class Order
    {
        [Key]
        public int Id { get; set; }
        public string UserId { get; set; }       // Identity luôn có, giữ non-null

        public int CourseId { get; set; }

        public decimal Amount { get; set; }

        public DateTime OrderDate { get; set; }

        public string Status { get; set; }       // luôn gán “Pending”/“Paid”/… nên giữ non-null

        public string? TransactionId { get; set; }   // <-- cho phép null ban đầu

        public string? PaymentMethod { get; set; }   // <-- có thể null

        public string? PaymentUrl { get; set; }      // <-- có thể null

        [ForeignKey("UserId")]
        public AppUserModel User { get; set; }
        [ForeignKey("CourseId")]
        public Course Course { get; set; }
    }

}