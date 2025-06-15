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
        public string UserId { get; set; }
        public int CourseId { get; set; }
        public decimal Amount { get; set; }
        public DateTime OrderDate { get; set; }
        public string Status { get; set; }            // Pending / Paid / Failed
        public string TransactionId { get; set; }     // vnp_TransactionNo
        public string PaymentUrl { get; set; }

        public string PaymentMethod { set; get; }

        [ForeignKey("UserId")]
        public AppUserModel User { get; set; }
        [ForeignKey("CourseId")]
        public Course Course { get; set; }
    }
}