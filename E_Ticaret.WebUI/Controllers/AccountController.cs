using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using E_Ticaret.Business.Abstarct;
using E_Ticaret.WebUI.Extensions;
using E_Ticaret.WebUI.Identity;
using E_Ticaret.WebUI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace E_Ticaret.WebUI.Controllers
{
    //[AutoValidateAntiforgeryToken]
    public class AccountController : Controller
    {
        

        private UserManager<ApplicationUser> _userManager;
        private SignInManager<ApplicationUser> _signInManager;
        private IEmailSender _emailSender;
        private ICartService _cartService;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IEmailSender emailSender, ICartService cartService)
        {       
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _cartService = cartService;
        }


        public IActionResult Register()//Üyelik İşlemi
        {
            return View(new RegisterModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult>  Register( RegisterModel registerModel)
        {
            if (!ModelState.IsValid)
            {
                return View(registerModel);
            }

            var user = new ApplicationUser
            {
                UserName = registerModel.UserName,
                Email = registerModel.Email,
                FullName = registerModel.FullName
            };

            var result = await _userManager.CreateAsync(user, registerModel.Password);

            if (result.Succeeded)//Giriş Başarılıysa
            {
                //Token oluşacak ve Maile gidecek ama kapalı bizde
                var tokenUser = await _userManager.GenerateEmailConfirmationTokenAsync(user);//Kullanıcı için token oluşacak
                var callBackUrl = Url.Action("ConfirmEmail", "Account", new
                {
                    userId = user.Id,
                    token = tokenUser
                });
                //Send Email
                await _emailSender.SendEmailAsync(registerModel.Email, "Hesabınızı onaylayınız.", $"Lütfen email hesabınızı onaylamak için linke <a href='https://localhost:44324{callBackUrl}'> tıklayınız</a>");

                //Uyarılar1
                TempData.Put("message", new ResultMessage()
                {
                    Title="Hesap Onayı",
                    Message="Eposta adresinize gelen link ile hesabınızı onaylayın.",
                    Css="warning"
                });

                return RedirectToAction("Login", "Account");
            }

            ModelState.AddModelError("", "Bilinmeyen bir hata oluştu lütfen tekrar deneyiniz");
            return View(registerModel);
        }
        

        public IActionResult Login(string ReturnUrl=null)//Login İşlemi //Return gelmiyorsa null olsun
        {

            return View(new LoginModel()
            {
                ReturnUrl = ReturnUrl
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]//Sayfadaki Token ı yakalar
        public async Task<IActionResult> Login(LoginModel model)//parametre olarak returnUrl yide aldık
        {
           
            if (!ModelState.IsValid)
            {
                return View(model);

            }

            //var user = await _userManager.FindByNameAsync(model.Username);//username ile giriş
            var user = await _userManager.FindByEmailAsync(model.Email);//Maile ile giriş
            if (user == null)//girdiği userneme ile ilişkli bir kullanıcı yoksa  
            {
                //ModelState.AddModelError("", "Bu kullanıcıyla ile daha önce hesap oluşturulmamış.");
                ModelState.AddModelError("", "Bu Email ile daha önce hesap oluşturulmamış.");
                return View(model);
            }
            //Mail onaylandıysa giriş yapacak
            if (!await _userManager.IsEmailConfirmedAsync(user))
            {
                ModelState.AddModelError("", "Lütfen Email hesabınıza giriş yaparak onaylayınız.");
                return View(model);
            }

            var result = await _signInManager.PasswordSignInAsync(user, model.Password, true, false);//1.false: şifre hatırlansın mı? 2.False: Yanlış girildiğindekitlensin mi?
            if (result.Succeeded)//Giriş başarılysa kullanıcı gitmek istediği yere gidecek returnUrl ile
            {

                return Redirect(model.ReturnUrl ?? "~/");

            }
            //ModelState.AddModelError("", "Kullanıcı adı yasa şifre yanlış");
            ModelState.AddModelError("", "Email yada şifre yanlış");
            return View(model);
        }



        public async Task<IActionResult> Logout()//Çıkış ilemi
        {
            await _signInManager.SignOutAsync();

            //Uyarılar2
            TempData.Put("message", new ResultMessage()
            {
                Title = "Oturum kapatıldı",
                Message = "Hesabınızdan güvenli bir şekilde çıkış yaptınız.",
                Css = "warning"
            });

            return Redirect("~/");
        }


        public async Task<IActionResult> ConfirmEmail(string userId,string token)//Mail gönderme
        {
            if (userId == null || token == null)
            {
                //TempData["message"] = "Geçersiz Token.";
                //Uyarılar3
                TempData.Put("message", new ResultMessage()
                {
                    Title = "Hesap onayı",
                    Message = "Hesap onayı için bilgileriniz yanlış ",
                    Css = "danger"
                });
                //return View();
                return RedirectToAction("~/");//Bunu Tempdata uyarılatınız yazınca verdm
            }
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                var result = await _userManager.ConfirmEmailAsync(user, token);
                if (result.Succeeded)
                {
                    //Burada Cart(sepet) işlemi olacak
                    _cartService.InitializeCart(user.Id);//kullanıcı için bir sepet oluşacak


                    //TempData["message"] = "Hesabınız onaylandı.";

                    //Uyarılar4
                    TempData.Put("message", new ResultMessage()
                    {
                        Title = "Hesap onayı",
                        Message = "Hesabınız başarıyla onaylandı ",
                        Css = "success"
                    });
                    //return View();
                    return RedirectToAction("Login");
                }

            }

            //TempData["message"] = "Hesanız onaylanmadı.";

            //Uyarılar5
            TempData.Put("message", new ResultMessage()
            {
                Title = "Hesap onayı",
                Message = "Hesabınız onaylanamadı. ",
                Css = "danger"
            });
            return View();

        }

        public IActionResult ForgotPassword()//Şifremi unuttum
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(string Email)//Şifremi unuttum post
        {
            if (string.IsNullOrEmpty(Email))//mail boşsa aynı sayfada olacak
            {
                //Uyarılar6
                TempData.Put("message", new ResultMessage()
                {
                    Title = "forgot Password",
                    Message = "Bilgileriniz hatalı",
                    Css = "danger"
                });
                return View();
            }
            var user =await _userManager.FindByEmailAsync(Email);

            if (user == null)
            {
                //Uyarılar7
                TempData.Put("message", new ResultMessage()
                {
                    Title = "forgot Password",
                    Message = "Eposta ile bir kullanıcı bulunamadı",
                    Css = "danger"
                });
                return View();
            }

            var sifre = await _userManager.GeneratePasswordResetTokenAsync(user);//şifreyi resetler

            var callBackUrl = Url.Action("ResetPassword", "Account", new
            {             
                token = sifre
            });
            //Send Email
            await _emailSender.SendEmailAsync(Email, "Rest Password", $"Parolanızı yenilemek için linke <a href='https://localhost:44324{callBackUrl}'> tıklayınız</a>");

            //Uyarılar8
            TempData.Put("message", new ResultMessage()
            {
                Title = "forgot Password",
                Message = "Parola yenilemek için hesabınıza mail gönderildiç",
                Css = "warning"
            });

            return RedirectToAction("Login", "Account");         
        }

        public IActionResult ResetPassword(string token)//şifre yenileme
        {
            if (token == null)
            {
                return RedirectToAction("Home", "Index");

            }
            var model = new ResetPasswordModel { Token = token };
            return View(model);
        }

        [HttpPost]
        public async  Task<IActionResult> ResetPassword(ResetPasswordModel model)//şifre yenileme
        {
            if (ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return RedirectToAction("Home", "Index");
            }

            var result = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("Account", "Login");
            }
            return View();
        }
    }
}
