﻿using Flim.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flim.Application.DTOs
{
    public class ConfirmBookingDTO
    {
        public int FilmId { get; set; }

        public ShowEnum category { get; set; }

        public DateOnly date {  get; set; }

    }
}