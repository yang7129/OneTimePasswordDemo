using OneTimePasswordDemo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OneTimePasswordDemo.Controllers
{
    public class DefaultController : Controller
    {
        // GET: Default
        private OTP otp;
        private string Secret = "TEST123456798";
        public ActionResult Index()
        {
            otp = new OTP(Secret);
            ViewBag.textCode = "Authentication code: " + otp + " (expires at " + otp.m_expiry.ToString("yyyy/MM/dd HH:mm:ss.fff") + " UTC)"; ;
            DateTime utcNow = DateTime.UtcNow;

            utcNow = utcNow.AddSeconds(30);
            otp = new OTP(Secret, utcNow);
            ViewBag.textCode1 = "Authentication code: " + otp + " (expires at " + otp.m_expiry.ToString("yyyy/MM/dd HH:mm:ss.fff") + " UTC)";

            utcNow = utcNow.AddSeconds(30);
            otp = new OTP(Secret, utcNow);
            ViewBag.textCode2 = "Authentication code: " + otp + " (expires at " + otp.m_expiry.ToString("yyyy/MM/dd HH:mm:ss.fff") + " UTC)";

            utcNow = utcNow.AddSeconds(30);
            otp = new OTP(Secret, utcNow);
            ViewBag.textCode3 = "Authentication code: " + otp + " (expires at " + otp.m_expiry.ToString("yyyy/MM/dd HH:mm:ss.fff") + " UTC)";

            utcNow = utcNow.AddSeconds(30);
            otp = new OTP(Secret, utcNow);
            ViewBag.textCode4 = "Authentication code: " + otp + " (expires at " + otp.m_expiry.ToString("yyyy/MM/dd HH:mm:ss.fff") + " UTC)";

            utcNow = utcNow.AddSeconds(30);
            otp = new OTP(Secret, utcNow);
            ViewBag.textCode5 = "Authentication code: " + otp + " (expires at " + otp.m_expiry.ToString("yyyy/MM/dd HH:mm:ss.fff") + " UTC)";

            utcNow = utcNow.AddSeconds(30);
            otp = new OTP(Secret, utcNow);
            ViewBag.textCode6 = "Authentication code: " + otp + " (expires at " + otp.m_expiry.ToString("yyyy/MM/dd HH:mm:ss.fff") + " UTC)";

            utcNow = utcNow.AddSeconds(30);
            otp = new OTP(Secret, utcNow);
            ViewBag.textCode7 = "Authentication code: " + otp + " (expires at " + otp.m_expiry.ToString("yyyy/MM/dd HH:mm:ss.fff") + " UTC)";

            utcNow = utcNow.AddSeconds(30);
            otp = new OTP(Secret, utcNow);
            ViewBag.textCode8 = "Authentication code: " + otp + " (expires at " + otp.m_expiry.ToString("yyyy/MM/dd HH:mm:ss.fff") + " UTC)";

            utcNow = utcNow.AddSeconds(30);
            return View();
        }
        public ActionResult Verify()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Verify(string Code)
        {
            ViewBag.Msg = "";
            if (string.IsNullOrEmpty(Code))
                ViewBag.Msg = "驗證失敗";
            else
            {
                Code = Code.Trim();
            }

            otp = new OTP(Secret);
            string VerifyCode = otp.ToString();
            if (VerifyCode == Code)
                ViewBag.Msg = "驗證成功";
            else
                ViewBag.Msg = "驗證失敗";

            return View();
        }
    }
}