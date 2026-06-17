$path = 'H:/steam/steamapps/common/Slay the Spire 2/data_sts2_windows_x86_64/sts2.dll'
$a = [Reflection.Assembly]::LoadFrom($path)
$a.GetTypes() | Where-Object { $_.FullName -match 'Stun|EndTurn|Retain|Flush|Hand|Turn|PlayerCmd|CombatState|PowerCmd' } | Select-Object FullName | Sort-Object FullName | Format-Table -AutoSize
