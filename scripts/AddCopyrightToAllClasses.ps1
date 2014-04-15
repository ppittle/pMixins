param($target = "C:\MyProject\trunk", $companyname = "Copacetic Software") 

#[System.Globalization.CultureInfo] 
$ci = [System.Globalization.CultureInfo]::GetCurrentCulture

# Full date pattern with a given CultureInfo 
# Look here for available String date patterns: http://www.csharp-examples.net/string-format-datetime/ 

$date = (Get-Date).ToString("F", $ci); 

# Header template 
$header = 
"//----------------------------------------------------------------------- 
// <copyright file=""{0}"" company=""{1}""> 
// Copyright (c) {1}.  
// <author>Philip Pittle</author> 
// <date>{2}</date> 
// Licensed under the Apache License, Version 2.0,
// you may not use this file except in compliance with this License.
//  
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an 'AS IS' BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright> 
//-----------------------------------------------------------------------`r`n"
 
  function Write-Header ($file) 
  { 
	# Get the file content as as Array object that contains the file lines 	
	$content = Get-Content $file 
	
	# Getting the content as a String 
	$contentAsString = $content | Out-String 
	
	# If content starts with //-- the skip it 
	if(!$contentAsString.StartsWith("//--"))
	{ 	
		# Splitting the file path and getting the leaf/last part, that is, the file name 
		$filename = Split-Path -Leaf $file 
		
		# $fileheader is assigned the value of $header with dynamic values passed as parameters after -f 	
		$fileheader = $header -f $filename, $companyname, $date 
		
		# Writing the header to the file 
		Set-Content $file $fileheader -encoding UTF8 
		
		# Append the content to the file
		Add-Content $file $content 	
	}
} 

#Filter files getting only .cs ones and exclude specific file extensions 
Get-ChildItem $target -Filter *.cs -Exclude TemporaryGeneratedFile*.cs, *.Designer.cs,T4MVC.cs,*.generated.cs,*.ModelUnbinder.cs -recurse | % `
{ 
	<# For each file on the $target directory that matches the filter, let's call the Write-Header function defined above passing the file as parameter #> 
	Write-Header $_.PSPath.Split(":", 3)[2] 
} 