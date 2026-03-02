using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Lab.API.Model_Binding.Models
{
    public class TodoItem : IValidatableObject // 先實作模型驗證介面
    {
        public int Id { get; set; }

        [Required] // 必填
        [StringLength(10, ErrorMessage = "超過長度了!")] // 字串長度
        public string? Name { get; set; }

        [DataType(DataType.Date)] // 欄位型態
        [Display(Name = "開始日期")] // 欄位名稱
        public DateTime StartDate { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "結束日期")]
        public DateTime EndDate { get; set; }

        [Range(0, 9999)] // 值的範圍
        public decimal Price { get; set; }

        //public bool? isComplete { get; set; }

        //public string? Password { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            // 先拿到 Db 的服務
            TodoContext _todoContext = (TodoContext)
                validationContext.GetService(typeof(TodoContext));

            bool isDistinct = _todoContext.TodoItems.Any(x => x.Name == Name);

            if (_todoContext != null)
            {
                if (isDistinct)
                {
                    // 因為回傳錯誤訊息是 IEnumerable 會有多個 , 所以用 yield 來疊加訊息再一次回傳
                    yield return new ValidationResult("名稱重複!");
                }

                if (StartDate > EndDate)
                {
                    yield return new ValidationResult("開始日期不能大於結束日期!");
                }
            }
        }

        // 自訂 DateRange 物件
        public class DateRange : IParsable<DateRange>
        {
            public DateOnly? From { get; set; } // 開始日期
            public DateOnly? To { get; set; } // 結束日期

            public static DateRange Parse(string value, IFormatProvider? provider)
            {
                if (!TryParse(value, provider, out var result))
                {
                    throw new ArgumentException("沒辦法轉換", nameof(value));
                }

                return result;
            }

            // 自訂一個 TryParse 方法
            public static bool TryParse(
                string? value,
                IFormatProvider? provider,
                out DateRange result
            )
            {
                // 預設回傳空的物件，而不是 null，避免 Action 報錯
                result = new DateRange();

                if (string.IsNullOrWhiteSpace(value))
                    return true;

                var segments = value.Split(
                    ',',
                    StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries
                );

                if (
                    segments.Length >= 1
                    && DateOnly.TryParse(segments[0], provider, out var fromDate)
                )
                {
                    result.From = fromDate;
                }

                if (
                    segments.Length >= 2
                    && DateOnly.TryParse(segments[1], provider, out var toDate)
                )
                {
                    result.To = toDate;
                }

                return true; // 回傳 true 讓 Model Binding 成功接手物件
            }
        }
    }
}
