   M S B A M L   `   `   `  ����  pokercsharp(    "pokercsharp.ui.InputGraphBoxDialog   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   -   5          � Title$  	Decisions��5   	   $	��200��5   	   $	��150��5   	    � SizeToContent$ WidthAndHeight=�5   	    � 
ResizeMode$ NoResize=�5	   	    � ShowInTaskbar$ False��5
   	    � WindowStartupLocation$ CenterOwner=�5   	    +� FocusedElement 5   	   ��  �� ElementName$ leftMarginTextBox��.��5      � $��10q�6    � 	Resources 5   
    �� 
TargetType	 �� Property
 �� Value��%v  '��       '�� v     '�� �     �� 5      " ��6   .�5      �� 	 � ��$
 0,3,5,5q�6+   �� 5      	 � ��$
 0,0,0,5q�6,   5      �� 5      " ��6   .�5      �� 	 � ��$
 0,0,0,5q�6+   5      �� 5      " ��6   .�5      �� 	 � ��$
 70��6*   �� 5      	 � ��$
 25��6+   �� 5      	 � ��$
 5,0,0,0q�6+   5      5       � ColumnDefinitions 5   
   �� 5      $
��Auto �6   �� 5        � RowDefinitions 5#   
   �� 5$      $
��Auto �6   �� 5%      $
��Auto �6   �� 5&      $
��Auto �6   �� 5'      $
��Auto �6   �� 5(      $
��Auto �6   �� 5)      .+�5-   
   �� $��0��6   $��0��6    .��6-   Raise:65   �� 5.   
   -   5        $��Raise��5.      $��1��6   $��0��6/   �� 50   
   $��0��6   $��1��6    .��6-   Call:64   �� 51   
   -   5        $
��Call��51      $��1��6   $��1��6.   �� 53   
   $��0��6   $��2��6    .��6-   Fold:64   �� 54   
   -   5        $
��Fold��54      $��1��6   $��2��6.   �� 57   
   $��0��6   $��2��6%   $��4��69    �� Orientation$ 
Horizontal=�6F   $��Right=�6_   .+�58      �� -   5        $��okButton��58       �� 	IsDefault$
 True��6<   .��6M   OK6Q   �� 59      -   5        $��cancelButton��59       �� IsCancel$
 True��6)   .��69   Cancel6A   5:      5;      5<      