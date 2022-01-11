
set /p msg= "Commit message:"
echo %cmsg%

git commit -m msg
git status 
pause
	