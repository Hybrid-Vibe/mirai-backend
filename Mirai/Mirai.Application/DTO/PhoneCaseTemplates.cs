using System;
using System.Collections.Generic;
using System.Text;

namespace Mirai.Application.DTO
{
    public static class PhoneCaseTemplates
    {
        public static readonly Dictionary<string, PhoneCaseTemplate> Map = new()
        {
            // ===================== iPhone 12 =====================
            ["iphone-12"] = new()
            {
                Model = "iPhone 12",
                Slug = "iphone-12",
                AspectRatio = "3:4",
                CameraHint = "dual camera diagonal top-left, small square bump",
                SafeZoneHint = "keep top-left corner empty for camera cutout"
            },

            ["iphone-12-pro"] = new()
            {
                Model = "iPhone 12 Pro",
                Slug = "iphone-12-pro",
                AspectRatio = "3:4",
                CameraHint = "triple camera triangular layout with LiDAR sensor",
                SafeZoneHint = "reserve top-left square camera cluster area"
            },

            ["iphone-12-pro-max"] = new()
            {
                Model = "iPhone 12 Pro Max",
                Slug = "iphone-12-pro-max",
                AspectRatio = "3:4",
                CameraHint = "large triple camera cluster with LiDAR, top-left heavy bump",
                SafeZoneHint = "avoid top-left large camera module zone"
            },

            // ===================== iPhone 13 =====================
            ["iphone-13"] = new()
            {
                Model = "iPhone 13",
                Slug = "iphone-13",
                AspectRatio = "3:4",
                CameraHint = "dual camera diagonal layout, slightly larger bump than iPhone 12",
                SafeZoneHint = "top-left diagonal camera cutout area reserved"
            },

            ["iphone-13-pro"] = new()
            {
                Model = "iPhone 13 Pro",
                Slug = "iphone-13-pro",
                AspectRatio = "3:4",
                CameraHint = "triple camera triangular layout, LiDAR sensor bottom right",
                SafeZoneHint = "top-left triple camera zone must remain empty"
            },

            ["iphone-13-pro-max"] = new()
            {
                Model = "iPhone 13 Pro Max",
                Slug = "iphone-13-pro-max",
                AspectRatio = "3:4",
                CameraHint = "large triple camera system with LiDAR sensor",
                SafeZoneHint = "large top-left camera cluster reserved area"
            },

            // ===================== iPhone 14 =====================
            ["iphone-14"] = new()
            {
                Model = "iPhone 14",
                Slug = "iphone-14",
                AspectRatio = "3:4",
                CameraHint = "dual camera diagonal layout, slightly refined bump",
                SafeZoneHint = "top-left diagonal dual camera zone"
            },

            ["iphone-14-plus"] = new()
            {
                Model = "iPhone 14 Plus",
                Slug = "iphone-14-plus",
                AspectRatio = "3:4",
                CameraHint = "dual camera diagonal layout on larger body",
                SafeZoneHint = "same dual camera top-left safe zone"
            },

            ["iphone-14-pro"] = new()
            {
                Model = "iPhone 14 Pro",
                Slug = "iphone-14-pro",
                AspectRatio = "3:4",
                CameraHint = "triple camera triangular layout with LiDAR sensor",
                SafeZoneHint = "dynamic island area top + triple camera top-left"
            },

            ["iphone-14-pro-max"] = new()
            {
                Model = "iPhone 14 Pro Max",

                Slug = "iphone-14-pro-max",

                AspectRatio = "3:4",

                CameraHint =
"iPhone 14 Pro Max camera module area at top-left corner, 43x43mm rounded square camera opening, raised camera protection border, triple lens cutout layout, flash and sensor holes",
                SafeZoneHint =
"leave a clean 5mm safety area around camera opening, no important artwork near camera, preserve raised camera border"
            },

            // ===================== iPhone 15 =====================
            ["iphone-15"] = new()
            {
                Model = "iPhone 15",
                Slug = "iphone-15",
                AspectRatio = "3:4",
                CameraHint = "dual camera diagonal layout with rounded edges",
                SafeZoneHint = "top-left dual camera zone reserved"
            },

            ["iphone-15-plus"] = new()
            {
                Model = "iPhone 15 Plus",
                Slug = "iphone-15-plus",
                AspectRatio = "3:4",
                CameraHint = "dual camera diagonal layout on large body",
                SafeZoneHint = "top-left camera cutout safe zone"
            },

            ["iphone-15-pro"] = new()
            {
                Model = "iPhone 15 Pro",
                Slug = "iphone-15-pro",
                AspectRatio = "3:4",
                CameraHint = "triple camera triangular layout with titanium frame",
                SafeZoneHint = "top-left triple camera + titanium edge clearance"
            },

            ["iphone-15-pro-max"] = new()
            {
                Model = "iPhone 15 Pro Max",
                Slug = "iphone-15-pro-max",
                AspectRatio = "3:4",
                CameraHint = "largest triple camera system with LiDAR sensor",
                SafeZoneHint = "large top-left camera module safe zone"
            },

            // ===================== iPhone 16 =====================
            ["iphone-16"] = new()
            {
                Model = "iPhone 16",
                Slug = "iphone-16",
                AspectRatio = "3:4",
                CameraHint = "dual camera vertical aligned layout (new generation)",
                SafeZoneHint = "top-left vertical dual camera cutout"
            },

            ["iphone-16-plus"] = new()
            {
                Model = "iPhone 16 Plus",
                Slug = "iphone-16-plus",
                AspectRatio = "3:4",
                CameraHint = "dual camera vertical layout on larger body",
                SafeZoneHint = "top-left vertical camera safe zone"
            },

            ["iphone-16-pro"] = new()
            {
                Model = "iPhone 16 Pro",
                Slug = "iphone-16-pro",
                AspectRatio = "3:4",
                CameraHint = "triple camera refined layout with LiDAR sensor",
                SafeZoneHint = "top-left triple camera refined cluster zone"
            },

            ["iphone-16-pro-max"] = new()
            {
                Model = "iPhone 16 Pro Max",
                Slug = "iphone-16-pro-max",
                AspectRatio = "3:4",
                CameraHint = "largest triple camera system with titanium body",
                SafeZoneHint = "top-left large professional camera cluster zone"
            }
        };
    }
}
