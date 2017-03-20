use strict;
use warnings;
use Cwd;
use Encode qw(encode decode);
my @WindowTitlesInEnglish;
my $FileToConvert=cwd()."//In.txt";
open(MYFILE1, $FileToConvert) || die "cannot open: $!";
while(<MYFILE1>)
{
	chomp($_);
	push(@WindowTitlesInEnglish,$_);
}
close(MYFILE1);
 
# my @WindowTitlesInEnglish=("Select a VM template",
							# "Name the new virtual machine",
							# "Locate the operating system installation media",
							# "Select a home server",
							# "Allocate processor and memory resources",
							# "Configure storage for the new VM",
							# "Configure networking on the new VM",
							# "Ready to create the new virtual machine");
							
my @TempWindowTitlesInEnglish=@WindowTitlesInEnglish;

my $OutFile=cwd().'/Mapped.txt';
#my $TTKFile=cwd().'/CHS.txt';
my $TTKFile=$ARGV[0];
$TTKFile=cwd()."//$TTKFile";
print "TTKFile -  $TTKFile\n";
open(MYFILE, '<:encoding(UTF-16)', $TTKFile) || die "cannot open: $!";

# open (OUT, '+>', $OutFile) || die "cannot open: $!";
# sleep 10;
while (<MYFILE>){

	#print OUT "$_\n";
	for (my $i=0;$i<scalar(@TempWindowTitlesInEnglish);$i++)
	{
		
		#print OUT "$keyVal2[0]   -   $TempWindowTitlesInEnglish[$i] \n";
		if($_=~/($TempWindowTitlesInEnglish[$i])/)
		{
			
			my @TempArr=split(/\t/,$_);
			#print OUT "After split  - $TempArr[0]\n";
			if($TempArr[0] eq $TempWindowTitlesInEnglish[$i])
			{
				#print OUT "Exact Match - $_\n";
				
				print OUT "\n";
				my $UnicodeString=$';
				my $EnString=$1;
				#repacle & in unicode string
				#$UnicodeString=~s/&//;
				my @TempArr1=split(/\t+/,$UnicodeString);
				$TempArr1[1]=~s/&//;
				print OUT "$EnString=$TempArr1[1]";
				#print OUT "unicpde - $TempArr1[1]\n";
				#Remove the matched window name from temp array
				splice(@TempWindowTitlesInEnglish,$i,1);
				#print OUT " TempWindowTitlesInEnglish - @TempWindowTitlesInEnglish\n";
				last;
			}
				
		}
		
	}
	
}
print OUT "\n";
if(@TempWindowTitlesInEnglish)
	{
	#Ceratin strings not available in ttk files are hardcoded. Write the hardcoded strings to file
	foreach my $wnd (@TempWindowTitlesInEnglish)
		{
			#"=" not at the beginning of string. SHould be there at the middle
			if($wnd=~/[a-z]/i && $wnd!~/^=/&& $wnd=~/=/)
			{
				print OUT "$wnd\n";
			}
		}
	
	}