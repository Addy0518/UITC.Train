namespace Lab.API.Swagger.Models
{
    /// <summary>
    /// UserDTO 模型
    /// </summary>
    public class UserDTO
    {
        /// <summary>
        ///  姓名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 角色
        /// </summary>
        public string Role { get; set; }

        /// <summary>
        /// 信箱
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// 密碼
        /// </summary>
        public string Password { get; set; }
    }
}
