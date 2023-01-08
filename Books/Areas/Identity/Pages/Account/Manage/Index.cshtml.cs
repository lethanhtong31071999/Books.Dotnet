// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Books.DataAcess.Repository;
using Books.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BookTemp.Areas.Identity.Pages.Account.Manage
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IUnitOfWork _unit;

        public IndexModel(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            IUnitOfWork unit)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _unit = unit;
        }
        public string Username { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            [Phone]
            [Display(Name = "Phone number")]
            public string PhoneNumber { get; set; }
            [Required]
            public string Name { get; set; }
            [Display(Name = "Stress Address")]
            [Required]
            public string StreetAddress { get; set; }
            [Required]
            public string City { get; set; }
            [Required]
            public string State { get; set; }
            [Required]
            public string PostalCode { get; set; }
            [Required]
            [ValidateNever]
            public string Role { get; set; }
        }

        private async Task LoadAsync(IdentityUser user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            Username = userName;
            var userRole = _unit.UserRepo.GetRoleUserByUserId(user.Id);
            var applicationUser = _unit.UserRepo.GetFirstOrDefault(x => x.Id == user.Id, isTracked: false);
            Input = new InputModel
            {
                Name = applicationUser.Name,
                PhoneNumber = phoneNumber,
                StreetAddress = applicationUser.StreetAddress,
                City = applicationUser.City,
                State = applicationUser.State,
                PostalCode = applicationUser.PostalCode,
                Role = userRole.NormalizedName,
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            var applicationUser = _unit.UserRepo.GetFirstOrDefault(x => x.Id == user.Id, isTracked: false);          
            if (applicationUser == null)
            {
                StatusMessage = "Unexpected error when trying to set phone number.";
                return RedirectToPage();
            }
            applicationUser.Name = Input.Name;
            applicationUser.PhoneNumber = Input.PhoneNumber;
            applicationUser.StreetAddress = Input.StreetAddress;
            applicationUser.City = Input.City;
            applicationUser.State = Input.State;
            applicationUser.PostalCode = Input.PostalCode;
            _unit.UserRepo.Update(applicationUser);
            _unit.Save();
            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }
    }
}
