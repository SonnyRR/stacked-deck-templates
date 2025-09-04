$certPwd = New-Guid;

Write-Host "`n"
Write-Host "Here's the password for the DEV SSL cert in case you need to note it down:"
Write-Host $certPwd
Write-Host "`n"

$defaultProject = Join-Path -Path $PWD.Path -ChildPath "\src\StackedDeck.WebAPI.Template\StackedDeck.WebAPI.Template.csproj"

dotnet dev-certs https -ep "$env:APPDATA\ASP.NET\Https\StackedDeck.WebAPI.Template.pfx" -p $certPwd
dotnet user-secrets init -p $defaultProject
dotnet user-secrets set "Kestrel:Certificates:Development:Password" $certPwd -p $defaultProject

Remove-Item $MyInvocation.MyCommand.Path -Force
