using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Auth
{
    public class RegisterResponseDto
    {
        public bool IsSuccess { get; set; }
        public IReadOnlyList<string> Errors { get; set; }
    }
}
