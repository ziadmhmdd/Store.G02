﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.G02.Domain.Exceptions
{
    public class ValidationException(IEnumerable<string> errors) : Exception("Validation Errors")
    {
        public IEnumerable<string> Errors { get; } = errors;
    }
}
