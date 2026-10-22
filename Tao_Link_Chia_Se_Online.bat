@echo off
chcp 65001 >nul
title TAO DUONG LINK ONLINE CHO NGUOI KHAC TRUY CAP WEBSITE
echo ========================================================
echo       TAO DUONG LINK ONLINE CHO NGUOI KHAC TRUY CAP
echo ========================================================
echo.
echo LUU Y: Ban phai dang bat Website tren may truoc (cong 5230)!
echo.
echo Dang tao duong link Internet (HTTPS) mien phi...
echo Xin vui long cho khoang 5 giay...
echo.
echo (Sau khi hien dong chu https://xxxx.lhr.life, hay copy link do gui cho ban be)
echo (De dung chia se, chi can dong cua so nay lai)
echo ========================================================
echo.
ssh -o StrictHostKeyChecking=no -R 80:localhost:5230 nokey@localhost.run
pause
