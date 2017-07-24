Imports System.Threading.Tasks
Imports Microsoft.AspNet.Identity
Imports Microsoft.AspNet.Identity.EntityFramework
Imports Microsoft.AspNet.Identity.Owin
Imports Microsoft.Owin

' 設定此應用程式中使用的應用程式使用者管理員。UserManager 在 ASP.NET Identity 中定義且由應用程式中使用。
Public Class ApplicationUserManager
    Inherits UserManager(Of ApplicationUser)
    Public Sub New(store As IUserStore(Of ApplicationUser))
        MyBase.New(store)
    End Sub

    Public Shared Function Create(options As IdentityFactoryOptions(Of ApplicationUserManager), context As IOwinContext) As ApplicationUserManager
        Dim manager = New ApplicationUserManager(New UserStore(Of ApplicationUser)(context.[Get](Of ApplicationDbContext)()))
        ' 設定使用者名稱的驗證邏輯
        manager.UserValidator = New UserValidator(Of ApplicationUser)(manager) With {
          .AllowOnlyAlphanumericUserNames = False,
          .RequireUniqueEmail = True
        }
        ' 設定密碼的驗證邏輯
        manager.PasswordValidator = New PasswordValidator() With {
          .RequiredLength = 6,
          .RequireNonLetterOrDigit = True,
          .RequireDigit = True,
          .RequireLowercase = True,
          .RequireUppercase = True
        }
        ' 註冊雙因素驗證提供者。此應用程式使用手機和電子郵件來接收驗證碼以驗證使用者。
        ' 如需使用雙因素驗證的詳細資訊，請造訪 http://go.microsoft.com/fwlink/?LinkID=391935
        ' 您可以在這裡寫下自己的提供者和外掛程式。
        manager.RegisterTwoFactorProvider("PhoneCode", New PhoneNumberTokenProvider(Of ApplicationUser)() With {
          .MessageFormat = "Your security code is: {0}"
        })
        manager.RegisterTwoFactorProvider("EmailCode", New EmailTokenProvider(Of ApplicationUser)() With {
          .Subject = "SecurityCode",
          .BodyFormat = "Your security code is: {0}"
        })
        manager.EmailService = New EmailService()
        manager.SmsService = New SmsService()
        Dim dataProtectionProvider = options.DataProtectionProvider
        If dataProtectionProvider IsNot Nothing Then
            manager.UserTokenProvider = New DataProtectorTokenProvider(Of ApplicationUser)(dataProtectionProvider.Create("ASP.NET Identity"))
        End If
        Return manager
    End Function
End Class

Public Class EmailService
    Implements IIdentityMessageService
    Public Function SendAsync(message As IdentityMessage) As Task Implements IIdentityMessageService.SendAsync
        ' 將您的電子郵件服務外掛到這裡以傳送電子郵件。
        Return Task.FromResult(0)
    End Function
End Class

Public Class SmsService
    Implements IIdentityMessageService
    Public Function SendAsync(message As IdentityMessage) As Task Implements IIdentityMessageService.SendAsync
        ' 將您的 SMS 服務外掛到這裡以傳送簡訊。
        Return Task.FromResult(0)
    End Function
End Class
