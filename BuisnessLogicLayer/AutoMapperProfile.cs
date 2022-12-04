﻿using AutoMapper;
using BuisnessLogicLayer.Models;
using DataAccessLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace BuisnessLogicLayer
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<AppFileData, FileDataModel>()
                //.ForMember(dm => dm.Name, d => d.MapFrom(e => e.UntrustedName))
                .ForMember(dm => dm.Name, d => d.MapFrom(e => WebUtility.HtmlEncode(e.UntrustedName)))
                .ForMember(dm => dm.UploadDateTime, d => d.MapFrom(e => e.UploadDT))
                .ForMember(dm => dm.OwnerId, d => d.MapFrom(e => e.OwnerId))
                .ForMember(dm => dm.OwnerName, d => d.MapFrom(e => e.OwnerNav == null ? string.Empty : e.OwnerNav.UserName))
                .ForMember(dm => dm.ShortLink, d => d.MapFrom(e => e.ShortLinkNav == null ? string.Empty : e.ShortLinkNav.Link))
                .ForMember(dm => dm.Viewers, d => d.MapFrom(e => e.FileViewers != null && e.FileViewers.Any()
                    //? e.FileViewers.Select(fv => new { fv.Id, Name = fv.UserName, fv.Email }) : null))
                    ? e.FileViewers.Select(fv => new UserModel
                    {
                        Name = fv.UserName,
                        Email = fv.Email,
                        Id = fv.Id,
                        Concurrency = fv.ConcurrencyStamp
                    }) : null))
                .ForMember(dm => dm.Content, d => d.MapFrom(e => e.AppFileNav == null ? null : e.AppFileNav.Content));
            //.ReverseMap();

            CreateMap<FileDataModel, AppFileData>()
                .ForMember(fd => fd.UntrustedName, dm => dm.MapFrom(e => e.Name))
                .ForMember(fd => fd.FileViewers, dm => dm.MapFrom(e => e.Viewers.Count > 0 ? e.Viewers.Select(fv => new AppUser { 
                    Id = fv.Id,
                    ConcurrencyStamp = fv.Concurrency
                }) : null))
                ;

            CreateMap<AppFileData, ShortFileDataModel>()
                .ForMember(dm => dm.Name, d => d.MapFrom(e => e.UntrustedName))
                .ForMember(dm => dm.UploadDateTime, d => d.MapFrom(e => e.UploadDT));

            CreateMap<AppUser, UserModel>()
                .ForMember(um => um.Concurrency, au => au.MapFrom(e => e.ConcurrencyStamp))
                .ForMember(um => um.Name, au => au.MapFrom(e => e.UserName))
                .ForMember(um => um.Email, au => au.MapFrom(e => e.Email));

            

        }
    }
}
