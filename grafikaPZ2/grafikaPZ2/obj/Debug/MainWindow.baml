   M S B A M L   `   `   `  ����E  AgrafikaPZ2, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'clr-namespace:grafikaPZ2
grafikaPZ2  5   	       grafikaPZ2.MainWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   *localclr-namespace:grafikaPZ2     5   	     � Title$  	PowerGrid��5   	    � WindowState$ 	Maximized=�5	   	   .��5
      ]� .+�5   
   � $	��Top=�6   $��41��6$   $��
0,0,-0.4,0q�60   ���#30a141.+�5      �� $��354,5,356,6q�6   $��ZOOM:��6)   $
��Bold"�69   �� 5      -   5         ��Name$ 
ZoomSlider��5       � Maximum$ 2Q�6)    � Value$
 1.25Q�65    � Minimum$ .5Q�6B   $	��Top=�6O   $��406,10,167,6q�6d   �� 5      -   5         ��Name$ 
LoadButton��5      $��	Load Grid��6)   $
��Left=�6=   $
��Bold"�6X   $��10,5,0,0q�6j   $	��Top=�6|   $	��164��6�   $��30��6�   6�   � 5      -   5         �Name$ progressBar��5      $
��Left=�6/   $��30��6J   $��	179,5,0,0q�6V   $	��Top=�6i   $	��170��6�   $ 0Q�6�   $	 100Q�6�   �� 5      -   5         ��Name$ progressText��5      	 �� TextAlignment$	 Center=�6/   $
��Left=�6F   $��
179,11,0,0q�6a   $	��Top=�6u   $
��Bold"�6�   $
��Wrap=�6�   $	��170��6�   $��29��6�   5      � 5   
   .+�5      �� $
��Auto=�6   $
��Auto=�6A   $��2��6d   $��
0,0,-0.4,0q�6x   .��5      �� -   5        $��	mapCanvas��5      $
��2200��6+   $
��2200��69   
 � LayoutTransform
 5      �� 5       �� ScaleX 6)   ��  �� ElementName$ 
ZoomSlider�� �� Path$ Value� �� ScaleY 5   &   �� $ 
ZoomSlider��$ Value�5      5      5      5      5      