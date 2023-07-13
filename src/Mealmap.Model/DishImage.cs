﻿using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;

namespace Mealmap.Model
{
    [Owned]
    public record DishImage
    {      
        public byte[] Content { get; init; }

        public string ContentType { get; init; }

        public DishImage(byte[] content, string contentType)
        {
            (Content, ContentType) = (content, contentType);
        }
    }
}